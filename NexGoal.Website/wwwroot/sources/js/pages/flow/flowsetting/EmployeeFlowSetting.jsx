import React, { Component, Fragment } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PAProcess } from '../../../constants';
import EmployeeFlowFilter from './EmployeeFlowFilter';
import EvaluatorList from './EvaluatorList';
import Sortable from './Sortable';

class EmployeeFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            humanResources: [],
            evaluators: [],
            reviewers: [],
            evaluatorSelected: null,
            groupOptions: [],
            employeeSelected: null,
            flowTemplateId: document.getElementById('FlowTemplateId').value,
            isHost: document.getElementById('IsHost').checked,
            showDeleteButton: false,
            showDeleteModal: false
        };

        this.handleOnEmployeeSelected = this.handleOnEmployeeSelected.bind(this);
        this.handleOnReceive = this.handleOnReceive.bind(this);
        this.handleOnDeactivate = this.handleOnDeactivate.bind(this);
        this.handleOnRemoveItem = this.handleOnRemoveItem.bind(this);
        this.handleSaveFlowSetting = this.handleSaveFlowSetting.bind(this);
        this.handleDeleteClick = this.handleDeleteClick.bind(this);
        this.handleDeleteFlowSetting = this.handleDeleteFlowSetting.bind(this);
    }

    componentWillMount() {
        this.getGroups();
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

    getFlowSetting(employeeId) {
        axios(commonFunctions.getRootUrl() + `EmployeeFlowSetting/GetById?employeeId=${employeeId}&flowTemplateId=${this.state.flowTemplateId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    humanResources: data.humanResources,
                    evaluators: data.evaluators,
                    reviewers: data.reviewers,
                    employeeSelected: {
                        employeeId: data.employeeId,
                        code: data.code,
                        nickName: data.nickName,
                        fullName: data.fullName,
                        base64Image: data.base64Image
                    },
                    showDeleteButton: data.humanResources.length > 0 || data.evaluators.length > 0 || data.reviewers.length > 0
                });
            }).catch(commonFunctions.axiosError);
    }

    handleOnEmployeeSelected(employeeId) {
        this.getFlowSetting(employeeId);
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
            axios(commonFunctions.getRootUrl() + `Flow/GetEmployeeById?employeeId=${employeeId}`)
                .then(response => {
                    const data = response.data;
                    let item = {
                        processId: processId,
                        evaluatorId: data.employeeId,
                        nickName: data.nickName,
                        fullName: data.fullName,
                        code: data.code,
                        base64Image: data.base64Image
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

    handleOnRemoveItem(sender, evaluatorId) {
        let items = [];
        let index;
        switch (sender) {
            case 'humanResources':
                items = this.state.humanResources.slice();
                index = items.map((item) => { return item.evaluatorId }).indexOf(evaluatorId);
                items.splice(index, 1);
                this.setState({ humanResources: items });
                break;

            case 'evaluators':
                items = this.state.evaluators.slice();
                index = items.map((item) => { return item.evaluatorId }).indexOf(evaluatorId);
                items.splice(index, 1);
                this.setState({ evaluators: items });
                break;

            case 'reviewers':
                items = this.state.reviewers.slice();
                index = items.map((item) => { return item.evaluatorId }).indexOf(evaluatorId);
                items.splice(index, 1);
                this.setState({ reviewers: items });
                break;
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
                url: commonFunctions.getRootUrl() + 'EmployeeFlowSetting/Edit',
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    flowTemplateId: this.state.flowTemplateId,
                    employeeId: this.state.employeeSelected.employeeId,
                    settings: settings
                }),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success('Updated successfully!');

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
            url: commonFunctions.getRootUrl() + 'EmployeeFlowSetting/Delete',
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                flowTemplateId: this.state.flowTemplateId,
                employeeId: this.state.employeeSelected.employeeId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Deleted successfully!');

                this.closeDeleteModal();

                this.setState({
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
        const employeeSelected = this.state.employeeSelected;

        let groupOptions = this.state.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        return (
            <div className="employee-flow">
                <EmployeeFlowFilter flowTemplateId={this.state.flowTemplateId} isHost={this.state.isHost}
                    hasSetting={this.state.showDeleteButton}
                    groupOptions={this.state.groupOptions}
                    onEmployeeSelected={this.handleOnEmployeeSelected} />
                <div className="boxtitle">PA Processing</div>
                <div className="row">
                    <div className="col-sm-3">
                        <EvaluatorList groupOptions={this.state.groupOptions} isHost={this.state.isHost} />
                    </div>
                    <div className="col-sm-9 flow-settings">
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
                                    <td className="col-hr">
                                        <Sortable id="humanResources" data={this.state.humanResources}
                                            enable={this.state.isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem} />
                                    </td>
                                    <td className="col-self">
                                        {employeeSelected &&
                                            <ul>
                                                <li className="alert alert-info">
                                                    <img src={employeeSelected.base64Image} />
                                                    <p>{`${employeeSelected.nickName} (${employeeSelected.fullName})`}</p>
                                                </li>
                                            </ul>
                                        }
                                    </td>
                                    <td className="col-evaluator">
                                        <Sortable id="evaluators" data={this.state.evaluators}
                                            enable={this.state.isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem} />
                                    </td>
                                    <td className="col-reviewer">
                                        <Sortable id="reviewers" data={this.state.reviewers}
                                            enable={this.state.isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem} />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        {this.state.isHost && employeeSelected &&
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
                            name="this employee's setting"
                            showModal={this.state.showDeleteModal}
                            closeModal={this.closeDeleteModal.bind(this)}
                            handleSubmit={this.handleDeleteFlowSetting} />
                    }
                </div>
            </div>
        );
    }
}

export default EmployeeFlowSetting;