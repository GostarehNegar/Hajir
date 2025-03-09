const { createApp } = Vue;

createApp({
    data() {
        return {
            products: [], // List of products fetched from the API
            searchQuery: "", // User's search input
            filteredProducts: [], // Filtered list of products
            selectedProduct: null, // Selected product
            isLoading: false, // Loading state
            error: "", // Error message
        };
    },
    methods: {
        // Filter products based on search query
        filterProducts() {
            if (this.searchQuery === "") {
                this.filteredProducts = [];
            } else {
                this.filteredProducts = this.products.filter((product) =>
                    product.name.toLowerCase().includes(this.searchQuery.toLowerCase())
                );
            }
        },
        // Select a product from the dropdown
        selectProduct(product) {
            this.selectedProduct = product;
            this.searchQuery = product.name; // Update the search input with the selected product name
            this.filteredProducts = []; // Clear the dropdown
        },
        // Fetch products from the API
        async fetchProducts() {
            this.isLoading = true;
            this.error = "";

            try {
                const response = await fetch("/api/datasheet");
                if (!response.ok) {
                    throw new Error("Failed to fetch products");
                }
                const data = await response.json();
                this.products = data; // Update the products list
            } catch (err) {
                this.error = "An error occurred while fetching products. Please try again later.";
                console.error(err);
            } finally {
                this.isLoading = false;
            }
        },
    },
    // Fetch products when the component is mounted
    mounted() {
        this.fetchProducts();
    },
}).mount("#app");