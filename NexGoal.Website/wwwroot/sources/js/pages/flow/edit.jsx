import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../shared/layouts/ToastLayout';
import commonFunctions from '../../common-functions';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EditInfo from './EditInfo';
import GroupFlowSetting from './flowsetting/GroupFlowSetting';
import EmployeeFlowSetting from './flowsetting/EmployeeFlowSetting';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1,
            isFirstLoadGroupTab: false,
            isFirstLoadPersonalTab: false
        };

        this.handleSelect = this.handleSelect.bind(this);
    }

    handleSelect(key) {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoadGroupTab) {
            this.setState({ isFirstLoadGroupTab: true });
        }

        if (key === 3 && !this.state.isFirstLoadPersonalTab) {
            this.setState({ isFirstLoadPersonalTab: true });
        }
    }

    handleBackClick() {
        window.location = commonFunctions.getRootUrl() + 'flow/index';
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Edit Evaluation Flow</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <a className="btn btn-cancel" onClick={this.handleBackClick}>
                            <i className="fa fa-angle-left"></i>Back to list
                        </a>
                    </div>
                    <div className="col-sm-12">
                        <Tabs id="tabs-nomination" animation={false}
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}>
                            <Tab eventKey={1} title="General Information">
                                <EditInfo />
                            </Tab>
                            <Tab eventKey={2} title="Team">
                                {this.state.isFirstLoadGroupTab && <GroupFlowSetting />}
                            </Tab>
                            <Tab eventKey={3} title="Personal">
                                {this.state.isFirstLoadPersonalTab && <EmployeeFlowSetting />}
                            </Tab>
                            <Tab eventKey={4} title="Flow List">
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('evaluationflow-wrapper'));