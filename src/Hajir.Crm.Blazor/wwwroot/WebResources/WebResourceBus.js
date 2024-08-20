class WebResourceBus {
	name = "";
	options = {}
	constructor(name, options) {
		this.name = name;
		this.options = options;
	}
	ping() {
		//this.options.callbackAdapter?.invokeMethodAsync(this.options.onMessageMethodName, {subject:"test"});
		return "pong";
	}
	initialize() {
		console.log("Initializing WebResourceBus", this.name, this.options.id);
		window.addEventListener("message", event => {
			console.log("ll", event.data);
			this.options.callbackAdapter?.invokeMethodAsync(this.options.onMessageMethodName, event.data);
		}, { capture: false });

		//parent.postMessage({ subject: "eval", expression: "parent.Xrm.Page.data.entity.getId()", id: "Date.now()" }, "*");


		return true;

	}
	async evaluate(id,expression) {
		parent.postMessage({ subject: "eval", expression: "parent.Xrm.Page.data.entity.getId()", id: id }, "*");
	}
}

export function createBus(name, options) {
	return new WebResourceBus(name, options);
}