import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EOQ from './eoq/index';
import EOY from './eoy/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: parseInt(document.getElementById('ActiveTab').value, 10),
            isFirstLoad: parseInt(document.getElementById('ActiveTab').value, 10) === 2
        };
    }

    handleSelect = key => {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    };

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('NominationPeriod')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tabs id="tabs"
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}
                        >

                            <Tab eventKey={1} title={getResource('EmployeeOfTheQuarter')}>
                                <EOQ />
                            </Tab>
                            <Tab eventKey={2} title={getResource('MostOutstandingEmployeeOfTheYear')}>
                                {
                                    this.state.isFirstLoad &&
                                    <EOY />
                                }
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('nomination-wrapper'));