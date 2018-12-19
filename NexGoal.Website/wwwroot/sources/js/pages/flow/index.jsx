import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../shared/layouts/ToastLayout';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import GroupFlowSetting from './flowsetting/GroupFlowSetting';
import EmployeeFlowSetting from './flowsetting/EmployeeFlowSetting';
import FlowList from './flowlist/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isHost: document.getElementById('IsHost').checked,
            activeKey: 1,
            isFirstLoadPersonalTab: false,
            isFirstLoadFlowListTab: false
        };
    }

    handleOnSelect = (key) => {
        this.setState({ activeKey: key });

        switch (key) {
            case 2:
                if (!this.state.isFirstLoadPersonalTab) {
                    this.setState({ isFirstLoadPersonalTab: true });
                }

                break;
            case 3:
                if (!this.state.isFirstLoadFlowListTab) {
                    this.setState({ isFirstLoadFlowListTab: true });
                }

                break;
        }
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('EvaluationFlow')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tabs id="tabs-flow"
                            activeKey={this.state.activeKey}
                            onSelect={this.handleOnSelect}
                        >
                            <Tab eventKey={1} title={getResource('Team')}>
                                <GroupFlowSetting
                                    isHost={this.state.isHost}
                                />
                            </Tab>
                            <Tab eventKey={2} title={getResource('Personal')}>
                                {
                                    this.state.isFirstLoadPersonalTab &&
                                    <EmployeeFlowSetting
                                        isHost={this.state.isHost}
                                    />
                                }
                            </Tab>
                            <Tab eventKey={3} title={getResource('FlowList')}>
                                {
                                    this.state.isFirstLoadFlowListTab &&
                                    <FlowList />
                                }
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('evaluationflow-wrapper'));