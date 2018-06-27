const path = require("path");
const { projectPath } = require("./constant");

const { clientInputPath, clientOutputPath } = projectPath;
const pagePath = path.join(clientInputPath, "pages");

module.exports = {
    client: {
        home: "./sources/js/home/index.jsx"
    }
};