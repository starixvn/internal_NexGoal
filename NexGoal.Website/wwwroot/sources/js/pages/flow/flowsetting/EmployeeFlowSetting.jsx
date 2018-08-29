import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PerformanceProcess } from '../../../constants';
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
            showDeleteButton: false,
            showDeleteModal: false
        };

        this.handleOnEmployeeSelected = this.handleOnEmployeeSelected.bind(this);
        this.handleOnReceive = this.handleOnReceive.bind(this);
        this.handleOnDeactivate = this.handleOnDeactivate.bind(this);
        this.handleOnRemoveItem = this.handleOnRemoveItem.bind(this);
        this.handleOnDeleteClick = this.handleOnDeleteClick.bind(this);
        this.handleSaveFlowSetting = this.handleSaveFlowSetting.bind(this);
        this.handleDeleteFlowSetting = this.handleDeleteFlowSetting.bind(this);
    }

    componentDidMount() {
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
        axios(commonFunctions.getRootUrl() + `EmployeeFlowSetting/GetById?employeeId=${employeeId}&flowTemplateId=${this.props.flowTemplateId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    humanResources: data.humanResources,
                    evaluators: data.evaluator !== null ? [data.evaluator] : [],
                    reviewers: data.reviewer !== null ? [data.reviewer] : [],
                    employeeSelected: {
                        employeeId: data.employeeId,
                        code: data.code,
                        nickName: data.nickName,
                        fullName: data.fullName,
                        base64Image: data.base64Image
                    },
                    showDeleteButton: data.humanResources.length > 0 || data.evaluator !== null || data.reviewer !== null
                });
            }).catch(commonFunctions.axiosError);
    }

    handleOnEmployeeSelected(employeeId) {
        this.getFlowSetting(employeeId);
    }

    handleAddItem(toIndex, employeeId, sender) {
        let items = this.state[sender].slice();
        let processId;

        switch (sender) {
            case 'humanResources':
                processId = PerformanceProcess.HumanResource;
                break;

            case 'evaluators':
                processId = PerformanceProcess.Evaluator;
                break;

            case 'reviewers':
                processId = PerformanceProcess.Reviewer;
                break;
        }

        let index = items.map((item) => { return item.evaluatorId; }).indexOf(employeeId);
        if (index < 0) {
            if ((sender === 'evaluators' || sender === 'reviewers') && items.length > 0) {
                toast.error('Evaluator or Reviewer is single setting!');
            } else {
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
                        this.setState({ [sender]: items });
                    }).catch(commonFunctions.axiosError);
            }
        } else {
            toast.error('This employee has already existed!');
        }
    }

    handleSortItems(fromIndex, toIndex, sender) {
        let items = this.state[sender].slice();
        items.splice(toIndex, 0, items.splice(fromIndex, 1)[0]);
        this.setState({ [sender]: items });
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
        let items = this.state[sender].slice();
        let index = items.map((item) => { return item.evaluatorId; }).indexOf(evaluatorId);
        items.splice(index, 1);
        this.setState({ [sender]: items });
    }

    handleSaveFlowSetting() {
        if (this.state.humanResources.length === 0
            || this.state.evaluators.length === 0
            || this.state.reviewers.length === 0) {
            toast.error('Please setting flow for Human Resource, Evaluator and Reviewer.');
        } else {
            let settings = [];

            let items = this.state.humanResources;
            for (let i = 0; i < items.length; i++) {
                let item = {
                    processId: items[i].processId,
                    evaluatorId: items[i].evaluatorId,
                    priority: i
                };

                settings.push(item);
            }

            settings.push({
                processId: this.state.evaluators[0].processId,
                evaluatorId: this.state.evaluators[0].evaluatorId,
                priority: 0
            });

            settings.push({
                processId: this.state.reviewers[0].processId,
                evaluatorId: this.state.reviewers[0].evaluatorId,
                priority: 0
            });

            axios({
                url: commonFunctions.getRootUrl() + 'EmployeeFlowSetting/Edit',
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    flowTemplateId: this.props.flowTemplateId,
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

    handleOnDeleteClick() {
        this.setState({
            showDeleteModal: true
        });
    }

    handleOnCloseModal() {
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
                flowTemplateId: this.props.flowTemplateId,
                employeeId: this.state.employeeSelected.employeeId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Deleted successfully!');

                this.handleOnCloseModal();

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
        const isHost = this.props.isHost;

        let groupOptions = this.state.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        return (
            <div className="employee-flow">
                <EmployeeFlowFilter flowTemplateId={this.props.flowTemplateId} isHost={isHost}
                    hasSetting={this.state.showDeleteButton}
                    groupOptions={this.state.groupOptions}
                    onEmployeeSelected={this.handleOnEmployeeSelected}
                />
                <div className="boxtitle">PA Processing</div>
                <div className="row">
                    <div className="col-sm-3">
                        <EvaluatorList groupOptions={this.state.groupOptions} isHost={isHost} />
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
                                            enable={isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem}
                                        />
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
                                            enable={isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem}
                                        />
                                    </td>
                                    <td className="col-reviewer">
                                        <Sortable id="reviewers" data={this.state.reviewers}
                                            enable={isHost}
                                            onReceive={this.handleOnReceive}
                                            onDeactivate={this.handleOnDeactivate}
                                            onRemove={this.handleOnRemoveItem}
                                        />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        {isHost && employeeSelected &&
                            <Form inline>
                                <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}>Submit</Button>
                                {this.state.showDeleteButton &&
                                    <Button type="button" bsStyle="primary" onClick={this.handleOnDeleteClick}>Delete</Button>}
                            </Form>
                        }
                    </div>
                    {
                        this.state.showDeleteModal &&
                        <ConfirmationModal
                            action="delete"
                            name="this employee's setting"
                            showModal={this.state.showDeleteModal}
                            closeModal={this.handleOnCloseModal.bind(this)}
                            handleSubmit={this.handleDeleteFlowSetting}
                        />
                    }
                </div>
            </div>
        );
    }
}

export default EmployeeFlowSetting;