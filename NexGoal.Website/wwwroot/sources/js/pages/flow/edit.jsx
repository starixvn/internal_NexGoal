import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../shared/layouts/ToastLayout';
import { resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EditInfo from './EditInfo';
import GroupFlowSetting from './flowsetting/GroupFlowSetting';
import EmployeeFlowSetting from './flowsetting/EmployeeFlowSetting';
import FlowList from './flowlist/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            flowTemplateId: document.getElementById('FlowTemplateId').value,
            isHost: document.getElementById('IsHost').checked,
            activeKey: 1,
            isFirstLoadGroupTab: false,
            isFirstLoadPersonalTab: false,
            isFirstLoadFlowListTab: false
        };

        this.handleOnSelect = this.handleOnSelect.bind(this);
    }

    handleOnSelect(key) {
        this.setState({ activeKey: key });

        switch (key) {
            case 2:
                if (!this.state.isFirstLoadGroupTab) {
                    this.setState({ isFirstLoadGroupTab: true });
                }

                break;
            case 3:
                if (!this.state.isFirstLoadPersonalTab) {
                    this.setState({ isFirstLoadPersonalTab: true });
                }

                break;
            case 4:
                if (!this.state.isFirstLoadFlowListTab) {
                    this.setState({ isFirstLoadFlowListTab: true });
                }

                break;
        }
    }

    handleOnBackClick() {
        window.location = resolveClientUrl('flow/index');
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('EditEvaluationFlow')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <a className="btn btn-cancel" onClick={this.handleOnBackClick}>
                            <i className="fa fa-angle-left" />{getResource('BackToList')}
                        </a>
                    </div>
                    <div className="col-sm-12">
                        <Tabs id="tabs-editflow" animation={false}
                            activeKey={this.state.activeKey}
                            onSelect={this.handleOnSelect}
                        >
                            <Tab eventKey={1} title={getResource('GeneralInformation')}>
                                <EditInfo flowTemplateId={this.state.flowTemplateId} isHost={this.state.isHost} />
                            </Tab>
                            <Tab eventKey={2} title={getResource('Team')}>
                                {this.state.isFirstLoadGroupTab &&
                                    <GroupFlowSetting flowTemplateId={this.state.flowTemplateId} isHost={this.state.isHost} />}
                            </Tab>
                            <Tab eventKey={3} title={getResource('Personal')}>
                                {this.state.isFirstLoadPersonalTab &&
                                    <EmployeeFlowSetting flowTemplateId={this.state.flowTemplateId} isHost={this.state.isHost} />}
                            </Tab>
                            <Tab eventKey={4} title={getResource('FlowList')}>
                                {this.state.isFirstLoadFlowListTab &&
                                    <FlowList flowTemplateId={this.state.flowTemplateId} />}
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('evaluationflow-wrapper'));