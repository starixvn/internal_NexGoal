const path = require("path");
const { projectPath } = require("./constant");

const { clientInputPath, clientOutputPath } = projectPath;
const pagePath = path.join(clientInputPath, "pages");

module.exports = {
    client: {
        cssmain: "./sources/styles/main.scss",
        //
        main: "./sources/js/main.js",
        home: "./sources/js/home/index.jsx"
    }
};