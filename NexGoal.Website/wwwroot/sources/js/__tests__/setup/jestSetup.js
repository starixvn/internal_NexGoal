document.getElementById = (id) => {
    if (id === "RootURL") {
        let input = document.createElement("input", { value: "/" });
        return input;
    }
};

window._virtualConsole.removeAllListeners("jsdomError");

window._virtualConsole.addListener("jsdomError", (error) => {
    if (error.type !== "not implemented" && originalListener) {
        originalListener(error);
    }
});