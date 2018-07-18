import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';
import { ToastContainer } from 'react-toastify';

import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EOQ from './eoq/index';
import EOY from './eoy/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.handleSelect = this.handleSelect.bind(this);

        this.state = {
            activeKey: 1,
            isFirstLoad: false
        };
    }

    handleSelect(key) {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Nomination Management</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tabs id="tabs-nomination" animation={false}
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}>

                            <Tab eventKey={1} title="Employee Of The Quarter">
                                <EOQ />
                            </Tab>
                            <Tab eventKey={2} title="Employee Of The Year">
                                {this.state.isFirstLoad && < EOY />}
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('nomination-wrapper'));