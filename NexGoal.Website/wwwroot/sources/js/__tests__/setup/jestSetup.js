window._virtualConsole.removeAllListeners("jsdomError");

document.getElementById = (id) => {
    if (id === "RootURL") {
        let input = document.createElement("input", { value: "/" });
        return input;
    }
};

document.getElementByName = (name) => {
    let input1 = document.createElement("input", { value: "123" });
    let input2 = document.createElement("input", { value: "123" });
    let output = [];
    output.push(input1).push(input2);
    return output;
};