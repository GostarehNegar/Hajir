class XrmFrameAdapter {
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
		console.log("Initializing XrmFrameAdapter", this.name, this.options.id);
		window.addEventListener("message", event => {
			console.log("ll", event.data);
			this.options.callbackAdapter?.invokeMethodAsync(this.options.onMessageMethodName, event.data);
		}, { capture: false });

		//parent.postMessage({ subject: "eval", expression: "parent.Xrm.Page.data.entity.getId()", id: "Date.now()" }, "*");


		return true;

	}
	async evaluate(id, expression) {
		parent.postMessage({ subject: "eval", expression: expression, id: id }, "*");
	}
}

export function createInstance(name, options) {
	return new XrmFrameAdapter(name, options);
}