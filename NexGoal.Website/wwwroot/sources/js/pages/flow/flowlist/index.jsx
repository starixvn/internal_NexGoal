import React, { Component } from 'react';
import { Tabs, Tab } from 'react-bootstrap';
import axios from 'axios';

import commonFunctions from '../../../common-functions';
import GroupFlowSetting from './GroupFlowSetting';
import EmployeeFlowSetting from './EmployeeFlowSetting';

class FlowList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1,
            isFirstLoadPersonalTab: false,
            groupFlowSettings: [],
            employeeFlowSettings: []
        };

        this.handleOnSelect = this.handleOnSelect.bind(this);
    }

    componentDidMount() {
        this.getGroupFlowSetting();
    }

    handleOnSelect(key) {
        this.setState({ activeKey: key });

        switch (key) {
            case 1:
                this.getGroupFlowSetting();
                break;
            case 2:
                this.getEmployeeFlowSetting();
                if (!this.state.isFirstLoadPersonalTab) {
                    this.setState({ isFirstLoadPersonalTab: true });
                }

                break;
        }
    }

    getGroupFlowSetting() {
        axios.get(commonFunctions.getRootUrl() + `GroupFlowSetting/GetGroupFlowSetting?flowTemplateId=${this.props.flowTemplateId}`)
            .then(response => {
                this.setState({ groupFlowSettings: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getEmployeeFlowSetting() {
        axios.get(commonFunctions.getRootUrl() + `EmployeeFlowSetting/GetEmployeeFlowSetting?flowTemplateId=${this.props.flowTemplateId}`)
            .then(response => {
                this.setState({ employeeFlowSettings: response.data });
            }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Tabs id="tabs-flowlist" animation={false}
                activeKey={this.state.activeKey}
                onSelect={this.handleOnSelect}
            >
                <Tab eventKey={1} title="Team">
                    <GroupFlowSetting groupFlowSettings={this.state.groupFlowSettings} />
                </Tab>
                <Tab eventKey={2} title="Personal">
                    {
                        this.state.isFirstLoadPersonalTab &&
                        <EmployeeFlowSetting employeeFlowSettings={this.state.employeeFlowSettings} />
                    }
                </Tab>
            </Tabs>
        );
    }
}

export default FlowList;