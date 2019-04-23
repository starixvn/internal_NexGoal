import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
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
            selectedGroup: null,
            enableDeleteButton: false,
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
                    enableDeleteButton: data.evaluator !== null || data.reviewer !== null
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
                        this.setState({ enableDeleteButton: true, groupSettings: groupSettings });
                    } else {
                        this.setState({ enableDeleteButton: true });
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
                    enableDeleteButton: false
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
                <GroupList groups={this.state.groupSettings} onClick={this.handleSelectGroup} />
                <div className="flow-settings">
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>{getResource('Team')}</th>
                                <th className="evaluator">{getResource('Evaluator')}</th>
                                <th className="reviewer">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td className="col-self">
                                    {this.state.selectedGroup &&
                                        <div className="flow-item">
                                            <i className="avatar icon-draft-member" />
                                            <div className="personal-info">
                                                <span className="txt-bold">{this.state.selectedGroup.name}</span></div>
                                        </div>
                                    }
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
                                        <div className="flow-item">
                                            <img className="avatar" src={this.state.evaluator.avatar} />
                                            <div className="personal-info">
                                                <span className="txt-bold">{this.state.evaluator.nickName}</span>
                                                <p>{this.state.evaluator.fullName}</p>
                                            </div>
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
                                        <div className="flow-item">
                                            <img className="avatar" src={this.state.reviewer.avatar} />
                                            <div className="personal-info">
                                                <span className="txt-bold">{this.state.reviewer.nickName}</span>
                                                <p>{this.state.reviewer.fullName}</p>
                                            </div>
                                        </div>
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    {
                        isHost && this.state.selectedGroup &&
                        <div className="button-group" >
                            <Button type="button" disabled={!this.state.enableDeleteButton} className="btn btn-cancel" onClick={this.handleOnDeleteClick}>{getResource('Delete')}</Button>
                            <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}>{getResource('Submit')}</Button>
                        </div>
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