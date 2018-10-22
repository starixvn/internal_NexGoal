import { JSDOM } from "jsdom";
const dom = new JSDOM();
global.document = dom.window.document;
global.window = dom.window;

global.document.getElementById = (id) => {
    if (id === "RootURL") {
        let input = document.createElement("input", { value: "/" });
        return input;
    }
};