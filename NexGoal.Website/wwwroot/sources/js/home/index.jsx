import React, { Component } from "react";
import ReactDOM from "react-dom";

import AppLayout from "../AppLayout";

export default class App extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <AppLayout>
                <h1>Hello</h1>
            </AppLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById("main-container"));