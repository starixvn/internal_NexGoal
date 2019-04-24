import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
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
            activeKey: 1,
            isFirstLoadPersonalTab: false,
            isFirstLoadFlowListTab: false,
            groupOptions: []
        };

        this.isHost = document.getElementById('IsHost').checked;
        this.employeeOptions = [];
    }

    componentDidMount() {
        var promises = [
            axios(resolveClientUrl('common/GetEmployees')),
            axios(resolveClientUrl('Common/GetGroups'))

        ];

        Promise.all(promises)
            .then(([employeeResponse, groupResponse]) => {
                this.employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                this.groupOptions = groupResponse.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });
                this.groupOptions.unshift({ value: '', label: '-- All --' });

                this.setState({ activeKey: 1 });
            }).catch(axiosError);
    }

    handleOnSelect = key => {
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
                <div className="row content-wrapper">
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
                                    isHost={this.isHost}
                                    groupOptions={this.groupOptions}
                                    employeeOptions={this.employeeOptions}

                                />
                            </Tab>
                            <Tab eventKey={2} title={getResource('Personal')}>
                                {
                                    this.state.isFirstLoadPersonalTab &&
                                    <EmployeeFlowSetting
                                        isHost={this.isHost}
                                        employeeOptions={this.employeeOptions}
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