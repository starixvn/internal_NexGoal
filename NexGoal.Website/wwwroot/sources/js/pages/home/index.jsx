import React, { Component } from "react";
import ReactDOM from "react-dom";
import PersonalTimeline from "./PersonalTimeline";

export default class App extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <PersonalTimeline />
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById("main-container"));