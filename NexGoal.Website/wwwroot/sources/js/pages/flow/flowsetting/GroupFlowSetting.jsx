import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PAProcess } from '../../../constants';
import EmployeeList from './EmployeeList';
import Sortable from './Sortable';

class GroupFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSettings: [],
            humanResources: [],
            evaluators: [],
            reviewers: [],
            evaluatorSelected: null,
            groupOptions: [],
            groupSelected: { value: '', label: '-- All --' },
            flowTemplateId: document.getElementById('FlowTemplateId').value,
            isHost: document.getElementById('IsHost').checked,
            showDeleteButton: false,
            showDeleteModal: false
        };

        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleOnReceive = this.handleOnReceive.bind(this);
        this.handleOnDeactivate = this.handleOnDeactivate.bind(this);
        this.handleSaveFlowSetting = this.handleSaveFlowSetting.bind(this);
        this.handleDeleteClick = this.handleDeleteClick.bind(this);
        this.handleDeleteFlowSetting = this.handleDeleteFlowSetting.bind(this);
    }

    componentWillMount() {
        this.getGroups();
        this.getGroupHasSetting();
    }

    getGroups() {
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const data = response.data;

                this.setState({
                    groupOptions: data
                });
            }).catch(commonFunctions.axiosError);
    }

    getGroupHasSetting() {
        axios(commonFunctions.getRootUrl() + `GroupFlowSetting/GetGroupHasSetting?flowTemplateId=${this.state.flowTemplateId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    groupSettings: data
                });
            }).catch(commonFunctions.axiosError);
    }

    getFlowSetting(groupId) {
        axios(commonFunctions.getRootUrl() + `GroupFlowSetting/GetById?groupId=${groupId}&flowTemplateId=${this.state.flowTemplateId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    humanResources: data.humanResources,
                    evaluators: data.evaluators,
                    reviewers: data.reviewers,
                    showDeleteButton: data.humanResources.length > 0 || data.evaluators.length > 0 || data.reviewers.length > 0
                });
            }).catch(commonFunctions.axiosError);
    }

    handleGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption
        });

        this.getFlowSetting(selectedOption.value);
    }

    handleAddItem(toIndex, employeeId, sender) {
        let items = [];
        let processId;

        switch (sender) {
            case 'humanResources':
                items = this.state.humanResources.slice();
                processId = PAProcess.HumanResource;
                break;

            case 'evaluators':
                items = this.state.evaluators.slice();
                processId = PAProcess.Evaluator;
                break;

            case 'reviewers':
                items = this.state.reviewers.slice();
                processId = PAProcess.Reviewer;
                break;
        }

        if (items.map((item) => { return item.evaluatorId; }).indexOf(employeeId) < 0) {
            axios(commonFunctions.getRootUrl() + `GroupFlowSetting/GetEmployeeById?employeeId=${employeeId}`)
                .then(response => {
                    const data = response.data;
                    let item = {
                        processId: processId,
                        evaluatorId: data.employeeId,
                        nickName: data.nickName,
                        fullName: data.fullName
                    };

                    items.splice(toIndex, 0, item);

                    switch (sender) {
                        case 'humanResources':
                            this.setState({ humanResources: items });
                            break;

                        case 'evaluators':
                            this.setState({ evaluators: items });
                            break;

                        case 'reviewers':
                            this.setState({ reviewers: items });
                            break;
                    }
                }).catch(commonFunctions.axiosError);
        } else {
            toast.error('This employee has already existed!');
        }
    }

    handleSortItems(fromIndex, toIndex, sender) {
        let items = [];
        switch (sender) {
            case 'humanResources':
                items = this.state.humanResources.slice();
                items.splice(toIndex, 0, items.splice(fromIndex, 1)[0]);
                this.setState({ humanResources: items });
                break;

            case 'evaluators':
                items = this.state.evaluators.slice();
                items.splice(toIndex, 0, items.splice(fromIndex, 1)[0]);
                this.setState({ evaluators: items });
                break;

            case 'reviewers':
                items = this.state.reviewers.slice();
                items.splice(toIndex, 0, items.splice(fromIndex, 1)[0]);
                this.setState({ reviewers: items });
                break;
        }
    }

    handleOnReceive(event, ui) {
        this.setState({ evaluatorSelected: parseInt(ui.sender.attr('id'), 10) });
    }

    handleOnDeactivate(event, ui) {
        let fromIndex = this.state.evaluatorSelected ? -1 : ui.item.startPos;
        let toIndex = ui.item.parent('ul.sortable').children().index(ui.item);

        if (toIndex >= 0) {
            if (fromIndex >= 0) {
                this.handleSortItems(fromIndex, toIndex, ui.sender.attr('id'));
            } else {
                this.handleAddItem(toIndex, this.state.evaluatorSelected, ui.sender.attr('id'));
                ui.item.remove();
                this.setState({ evaluatorSelected: null });
            }
        }
    }

    handleSaveFlowSetting() {
        if (this.state.humanResources.length === 0
            || this.state.evaluators.length === 0
            || this.state.reviewers.length === 0) {
            toast.error('Please setting flow for Human Resource, Evaluator and Reviewer.');
        } else {
            let settings = [], items, i;

            items = this.state.humanResources;
            for (i = 0; i < items.length; i++) {
                let item = {
                    processId: items[i].processId,
                    evaluatorId: items[i].evaluatorId,
                    priority: i
                }

                settings.push(item);
            }

            items = this.state.evaluators;
            for (i = 0; i < items.length; i++) {
                let item = {
                    processId: items[i].processId,
                    evaluatorId: items[i].evaluatorId,
                    priority: i
                }

                settings.push(item);
            }

            items = this.state.reviewers;
            for (i = 0; i < items.length; i++) {
                let item = {
                    processId: items[i].processId,
                    evaluatorId: items[i].evaluatorId,
                    priority: i
                }

                settings.push(item);
            }

            axios({
                url: commonFunctions.getRootUrl() + 'GroupFlowSetting/Edit',
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    flowTemplateId: this.state.flowTemplateId,
                    groupId: this.state.groupSelected.value,
                    settings: settings
                }),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success('Updated successfully!');

                    let groupSettings = this.state.groupSettings.slice();
                    let groupIds = groupSettings.map((group) => {
                        return group.groupId;
                    });

                    if (groupIds.indexOf(this.state.groupSelected.value) === -1) {
                        groupSettings.push({ groupId: this.state.groupSelected.value, name: this.state.groupSelected.label });

                        this.setState({ groupSettings });
                    }

                    this.setState({ showDeleteButton: true });
                } else {
                    toast.error(data.message);
                }
            }).catch(commonFunctions.axiosError);
        }
    }

    handleDeleteClick() {
        this.setState({
            showDeleteModal: true
        });
    }

    closeDeleteModal() {
        this.setState({
            showDeleteModal: false
        });
    }

    handleDeleteFlowSetting() {
        axios({
            url: commonFunctions.getRootUrl() + 'GroupFlowSetting/Delete',
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                flowTemplateId: this.state.flowTemplateId,
                groupId: this.state.groupSelected.value
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Deleted successfully!');

                this.closeDeleteModal();

                let groupSettings = this.state.groupSettings.slice();
                let groupIds = groupSettings.map((group) => {
                    return group.groupId;
                });
                let index = groupIds.indexOf(this.state.groupSelected.value);
                groupSettings.splice(index, 1);

                this.setState({
                    groupSettings: groupSettings,
                    humanResources: [],
                    evaluators: [],
                    reviewers: [],
                    showDeleteButton: false
                });
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        let groupOptions = this.state.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const groupSetting = this.state.groupSettings.map((group) => {
            return group.name;
        }).join(', ');

        return (
            <div className="row">
                <div className="col-sm-3">
                    <EmployeeList groupOptions={this.state.groupOptions} isHost={this.state.isHost} />
                </div>
                <div className="col-sm-9 flow-settings">
                    <Form horizontal>
                        <FormGroup>
                            <label className="col-sm-3 control-label">Team have setting</label>
                            <div className="col-sm-9 group-has-setting">{groupSetting}</div>
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
                                <th>Evaluator</th>
                                <th>Reviewer</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <Sortable id="humanResources" data={this.state.humanResources}
                                        enable={this.state.isHost}
                                        onReceive={this.handleOnReceive}
                                        onDeactivate={this.handleOnDeactivate} />
                                </td>
                                <td>
                                    {this.state.groupSelected.value &&
                                        <ul>
                                            <li className="alert alert-info">
                                                <img />
                                                <p>Self</p>
                                            </li>
                                        </ul>
                                    }
                                </td>
                                <td>
                                    <Sortable id="evaluators" data={this.state.evaluators}
                                        enable={this.state.isHost}
                                        onReceive={this.handleOnReceive}
                                        onDeactivate={this.handleOnDeactivate} />
                                </td>
                                <td>
                                    <Sortable id="reviewers" data={this.state.reviewers}
                                        enable={this.state.isHost}
                                        onReceive={this.handleOnReceive}
                                        onDeactivate={this.handleOnDeactivate} />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    {this.state.isHost && this.state.groupSelected.value &&
                        <Form inline>
                            <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}>Submit</Button>
                            {this.state.showDeleteButton &&
                                <Button type="button" bsStyle="primary" onClick={this.handleDeleteClick}>Delete</Button>}
                        </Form>
                    }
                </div>
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this group's setting"
                        showModal={this.state.showDeleteModal}
                        closeModal={this.closeDeleteModal.bind(this)}
                        handleSubmit={this.handleDeleteFlowSetting} />
                }
            </div>
        );
    }
}

export default GroupFlowSetting;