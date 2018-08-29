import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import EditBasicInformation from './EditBasicInformation';
import EditProcessSetting from './EditProcessSetting';
import EditEmployeeSetting from './EditEmployeeSetting/EditEmployeeSetting';

export default class EditPeriod extends Component {
    constructor(props) {
        super(props);

        this.handleSelect = this.handleSelect.bind(this);

        this.state = {
            periodId: document.getElementById('periodId').value,
            activeKey: 1,
            loadProcessSettingTab: false,
            loadEmployeeTab: false
        };
    }

    handleSelect(key) {
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
            <div className="row">
                <Breadcrumb>
                    <li className="current">Edit Evaluation Period</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <a href='/period' className="btn btn-cancel"><i className="fa fa-angle-left" />Back to list</a>
                    <Tabs id="tabs" activeKey={this.state.activeKey} onSelect={this.handleSelect}>
                        <Tab eventKey={1} title="Basic Information">
                            <EditBasicInformation periodId={this.state.periodId} />
                        </Tab>
                        <Tab eventKey={2} title="Process Setting">
                            {this.state.loadProcessSettingTab &&
                                <EditProcessSetting periodId={this.state.periodId} />
                            }
                        </Tab>
                        <Tab eventKey={3} title="Employee Setting">
                            {this.state.loadEmployeeTab &&
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

ReactDOM.render(<EditPeriod />, document.getElementById('edit-period-wrapper'));