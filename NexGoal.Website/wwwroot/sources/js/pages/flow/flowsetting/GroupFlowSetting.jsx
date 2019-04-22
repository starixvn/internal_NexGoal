import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PerformanceProcess } from '../../../constants';
import GroupList from './GroupList';

class GroupFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSettings: [],
            evaluator: null,
            reviewer: null,
            evaluatorSelected: null,
            selectedGroup: null,
            showDeleteButton: false,
            showDeleteModal: false
        };

        this.employeeOptions = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('common/GetEmployees')),
            axios(resolveClientUrl('GroupFlowSetting/GetGroups'))
        ];

        Promise.all(promises)
            .then(([employeeResponse, groupResponse]) => {
                this.employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });

                this.setState({
                    groupSettings: groupResponse.data
                });
            }).catch(axiosError);
    }

    getFlowSetting = groupId => {
        axios(resolveClientUrl(`GroupFlowSetting/GetById?groupId=${groupId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    evaluator: data.evaluator,
                    reviewer: data.reviewer,
                    showDeleteButton: data.evaluator !== null || data.reviewer !== null
                });
            }).catch(axiosError);
    }

    handleSelectGroup = selectedGroup => {
        this.setState({
            selectedGroup: selectedGroup
        });

        this.getFlowSetting(selectedGroup.groupId);
    }

    getEmployee = (employeeId, callback) => {
        axios(resolveClientUrl(`Flow/GetEmployeeById?employeeId=${employeeId}`))
            .then(response => {
                callback(response.data);
            }).catch(axiosError);
    }

    handleEvaluatorChange = selectedEvaluator => {
        this.getEmployee(selectedEvaluator.value, employee => {
            let evaluator = {
                ...this.state.evaluator,
                ...{
                    processId: PerformanceProcess.Evaluator,
                    evaluatorId: employee.employeeId,
                    nickName: employee.nickName,
                    fullName: employee.fullName,
                    code: employee.code,
                    avatar: employee.avatar
                }
            };
            this.setState({ evaluator });
        });
    };

    handleReviewerChange = selectedReviewer => {
        this.getEmployee(selectedReviewer.value, employee => {
            let reviewer = {
                ...this.state.reviewer,
                ...{
                    processId: PerformanceProcess.Reviewer,
                    evaluatorId: employee.employeeId,
                    nickName: employee.nickName,
                    fullName: employee.fullName,
                    code: employee.code,
                    avatar: employee.avatar
                }
            };
            this.setState({ reviewer });
        });
    };

    handleSortItems = (fromIndex, toIndex, sender) => {
        let items = this.state[sender].slice();
        items.splice(toIndex, 0, items.splice(fromIndex, 1)[0]);
        this.setState({ [sender]: items });
    }

    handleOnReceive = (event, ui) => {
        this.setState({ evaluatorSelected: parseInt(ui.sender.attr('id'), 10) });
    }

    handleOnDeactivate = (event, ui) => {
        const fromIndex = this.state.evaluatorSelected ? -1 : ui.item.startPos;
        const toIndex = ui.item.parent('ul.sortable').children().index(ui.item);

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

    handleOnRemoveItem = (sender, evaluatorId) => {
        let items = this.state[sender].slice();
        const index = items.findIndex(item => item.evaluatorId === evaluatorId);
        items.splice(index, 1);
        this.setState({ [sender]: items });
    }

    handleSaveFlowSetting = () => {
        if (!this.state.evaluator
            || !this.state.reviewer) {
            toast.error('Please setting flow for Evaluator and Reviewer.');
        } else {
            let settings = [];

            settings.push({
                processId: this.state.evaluator.processId,
                evaluatorId: this.state.evaluator.evaluatorId,
                priority: 0
            });

            settings.push({
                processId: this.state.reviewer.processId,
                evaluatorId: this.state.reviewer.evaluatorId,
                priority: 0
            });

            axios({
                url: resolveClientUrl('GroupFlowSetting/Edit'),
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    groupId: this.state.selectedGroup.groupId,
                    settings: settings
                }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                const data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('UpdatedSuccessfully'));

                    let groupSettings = this.state.groupSettings.slice();
                    let selectedGroup = groupSettings.find(g => g.groupId === this.state.selectedGroup.groupId);

                    if (!selectedGroup.hasSetting) {
                        selectedGroup.hasSetting = true;
                        this.setState({ showDeleteButton: true, groupSettings: groupSettings });
                    } else {
                        this.setState({ showDeleteButton: true });
                    }
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        }
    }

    handleOnDeleteClick = () => {
        this.setState({
            showDeleteModal: true
        });
    }

    handleOnCloseModal = () => {
        this.setState({
            showDeleteModal: false
        });
    }

    handleDeleteFlowSetting = () => {
        axios({
            url: resolveClientUrl('GroupFlowSetting/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                groupId: this.state.selectedGroup.groupId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.handleOnCloseModal();

                let groupSettings = this.state.groupSettings.slice();
                let selectedGroup = groupSettings.find(g => g.groupId === this.state.selectedGroup.groupId);
                selectedGroup.hasSetting = false;

                this.setState({
                    groupSettings: groupSettings,
                    evaluator: null,
                    reviewer: null,
                    showDeleteButton: false
                });
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const { isHost } = this.props;

        return (
            <div className="row group-flow">
                <div className="col-sm-3">
                    <GroupList groups={this.state.groupSettings} onClick={this.handleSelectGroup} />
                </div>
                <div className="col-sm-9 flow-settings">
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>{getResource('Team')}</th>
                                <th>{getResource('Evaluator')}</th>
                                <th>{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td className="col-self">

                                    {this.state.selectedGroup && this.state.selectedGroup.name}
                                </td>
                                <td className="col-evaluator">
                                    {isHost && this.state.selectedGroup &&
                                        < Select className="react-select-container" classNamePrefix="react-select"
                                            value={null} options={this.employeeOptions}
                                            isClearable={false} placeholder="Nickname or full name"
                                            onChange={this.handleEvaluatorChange}
                                        />
                                    }
                                    {this.state.evaluator &&
                                        <div>
                                            <img src={this.state.evaluator.avatar} />
                                            {this.state.evaluator.nickName}
                                        </div>}
                                </td>
                                <td className="col-reviewer">
                                    {isHost && this.state.selectedGroup &&
                                        <Select className="react-select-container" classNamePrefix="react-select"
                                            value={null} options={this.employeeOptions}
                                            isClearable={false} placeholder="Nickname or full name"
                                            onChange={this.handleReviewerChange}
                                        />
                                    }

                                    {this.state.reviewer &&
                                        <div>
                                            <img src={this.state.reviewer.avatar} />
                                            {this.state.reviewer.nickName}
                                        </div>
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    {
                        isHost && this.state.selectedGroup &&
                        <Form inline>
                            <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}>{getResource('Submit')}</Button>
                            {
                                this.state.showDeleteButton &&
                                <Button type="button" bsStyle="primary" onClick={this.handleOnDeleteClick}>{getResource('Delete')}</Button>
                            }
                        </Form>
                    }
                </div>
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this group's setting"
                        showModal={this.state.showDeleteModal}
                        onCloseModal={this.handleOnCloseModal}
                        onSubmit={this.handleDeleteFlowSetting}
                    />
                }
            </div >
        );
    }
}

export default GroupFlowSetting;