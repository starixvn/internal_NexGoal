import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import { resolveClientUrl } from '../../common-functions';
import EditBasicInformation from './EditBasicInformation';
import EditProcessSetting from './EditProcessSetting';
import EditEmployeeSetting from './EditEmployeeSetting/EditEmployeeSetting';

export default class Edit extends Component {
    constructor(props) {
        super(props);

        this.state = {
            periodId: document.getElementById('periodId').value,
            activeKey: 1,
            loadProcessSettingTab: false,
            loadEmployeeTab: false
        };
    }

    handleSelect = key => {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.loadProcessSettingTab) {
            this.setState({ loadProcessSettingTab: true });
        }

        if (key === 3 && !this.state.loadEmployeeTab) {
            this.setState({ loadEmployeeTab: true });
        }
    }

    render() {
        return (
            <div className="row content-wrapper">
                <Breadcrumb>
                    <li><a href={resolveClientUrl('period')}>{getResource('EvaluationPeriod')}</a></li>
                    <li className="current">{getResource('EditEvaluationPeriod')}</li>
                </Breadcrumb>
                <div className="col-sm-12">

                    <Tabs id="tabs" activeKey={this.state.activeKey} onSelect={this.handleSelect}>
                        <Tab eventKey={1} title="Basic Information">
                            <EditBasicInformation periodId={this.state.periodId} />
                        </Tab>
                        <Tab eventKey={2} title="Process Setting">
                            {
                                this.state.loadProcessSettingTab &&
                                <EditProcessSetting periodId={this.state.periodId} />
                            }
                        </Tab>
                        <Tab eventKey={3} title="Employee Setting">
                            {
                                this.state.loadEmployeeTab &&
                                <EditEmployeeSetting periodId={this.state.periodId} />
                            }
                        </Tab>
                    </Tabs>
                </div>
                <ToastLayout />

            </div>

        );
    }
}

ReactDOM.render(<Edit />, document.getElementById('edit-period-wrapper'));