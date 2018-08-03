import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';
import EmployeeList from './EmployeeList';
import Sortable from './Sortable';

class GroupFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groups: [],
            groupSelected: { value: '', label: '-- All --' },
            flowSetting: {
                humanResources: []
            },
            isHost: document.getElementById('IsHost').checked
        };

        this.handleGroupChange = this.handleGroupChange.bind(this);
    }

    componentWillMount() {
        this.getGroups();
    }

    getGroups() {
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const data = response.data;

                this.setState({
                    groups: data
                });
            }).catch(commonFunctions.axiosError);
    }

    handleGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption
        });
    }

    handleOnReceive(event, ui) {
        console.log('DOM changed!', event, ui);
    }

    handleOnDeactivate(event, ui) {
        console.log('DOM changed!', event, ui);
    }

    render() {
        const groupOptions = this.state.groups.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const flowSetting = this.state.flowSetting;

        return (
            <div className="row">
                <div className="col-sm-3">
                    <EmployeeList groupOptions={this.state.groups} />
                </div>
                <div className="col-sm-9 flow-settings">
                    <Form horizontal>
                        <FormGroup>
                            <label className="col-sm-3 control-label">Team have setting</label>
                            <div className="col-sm-9">
                                
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label">Apply to Team</label>
                            <div className="col-sm-5">
                                <Select value={this.state.groupSelected} options={groupOptions}
                                    clearable={false} onChange={this.handleGroupChange} />
                            </div>
                        </FormGroup>
                    </Form>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>Admin</th>
                                <th>Self</th>
                                <th>Project Manager</th>
                                <th>Evaluator</th>
                                <th>Reviewer</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><Sortable data={flowSetting.humanResources} onReceive={this.handleOnReceive} onDeactivate={this.handleOnDeactivate} /></td>
                                <td></td>
                                <td><Sortable data={flowSetting.humanResources} onReceive={this.handleOnReceive} onDeactivate={this.handleOnDeactivate} /></td>
                                <td><Sortable data={flowSetting.humanResources} onReceive={this.handleOnReceive} onDeactivate={this.handleOnDeactivate} /></td>
                                <td><Sortable data={flowSetting.humanResources} onReceive={this.handleOnReceive} onDeactivate={this.handleOnDeactivate} /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}

export default GroupFlowSetting;