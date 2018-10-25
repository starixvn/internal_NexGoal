window._virtualConsole.removeAllListeners("jsdomError");

document.getElementById = (id) => {
    if (id === "RootURL") {
        let input = document.createElement("input", { value: "/" });
        return input;
    }
};