import React, { Component } from 'react';
import ReactDOM from 'react-dom';

import Breadcrumb from '../../shared/components/Breadcrumb';

export default class App extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Nomination Management</li>
                </Breadcrumb>
                <div className="col-sm-12">
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('nomination-wrapper'));