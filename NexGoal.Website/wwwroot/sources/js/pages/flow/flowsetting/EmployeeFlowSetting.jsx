import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import Select from 'react-select';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PerformanceProcess } from '../../../constants';
import ConfiguredEmployeeList from './ConfiguredEmployeeList';

export default class EmployeeFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            member: null,
            evaluator: null,
            reviewer: null,
            selectedEmployeeId: null,
            showDeleteModal: false,
            configuredEmployees: [],
            unconfiguredEmployees: [],
            unconfiguredEmployeeOptions: [],
            submit: false
        };
    }

    componentDidMount() {
        this.getEmployeeHaveSetting();
        this.getEmployeeNotHaveSetting();
    }

    getEmployeeNotHaveSetting = () => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetEmployeeUnsetting`))
            .then(response => {
                this.setState({
                    unconfiguredEmployees: response.data,
                    unconfiguredEmployeeOptions: response.data.map(e => ({ value: e.employeeId, label: `${e.nickName} (${e.fullName})` }))
                });
            }).catch(axiosError);
    }

    getEmployeeHaveSetting = () => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetEmployeeHaveSetting`))
            .then(response => {
                this.setState({
                    configuredEmployees: response.data
                });
            }).catch(axiosError);
    }

    getFlowSetting = employeeId => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetById?employeeId=${employeeId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    evaluator: data.evaluator,
                    reviewer: data.reviewer,
                    member: {
                        employeeId: data.employeeId,
                        code: data.code,
                        nickName: data.nickName,
                        fullName: data.fullName,
                        avatar: data.avatar
                    }
                });
            }).catch(axiosError);
    }

    handleSelectEmployee = employee => {
        this.setState({ selectedEmployeeId: employee.employeeId });
        this.getFlowSetting(employee.employeeId);
    }

    getEmployee = (employeeId, callback) => {
        axios(resolveClientUrl(`Flow/GetEmployeeById?employeeId=${employeeId}`))
            .then(response => {
                callback(response.data);
            }).catch(axiosError);
    }

    handleMemberChange = member => {
        this.getEmployee(member.value, employee => {
            let member = {
                ...this.state.member,
                ...{
                    employeeId: employee.employeeId,
                    nickName: employee.nickName,
                    fullName: employee.fullName,
                    code: employee.code,
                    avatar: employee.avatar
                }
            };
            this.setState({ member });
        });
    };

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
        if (!this.state.submit) {
            this.setState({ submit: true });
        }

        if (!this.state.evaluator || !this.state.reviewer || !this.state.member) {
            toast.error('Please select Member, Evaluator and Reviewer.');
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
                url: resolveClientUrl('EmployeeFlowSetting/Edit'),
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    employeeId: this.state.member.employeeId,
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

                    if (this.state.selectedEmployeeId === null) {
                        this.getEmployeeHaveSetting();
                        this.getEmployeeNotHaveSetting();
                    }

                    this.clear();
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        }
    }

    clear = () => {
        this.setState({
            member: null,
            evaluator: null,
            reviewer: null,
            selectedEmployeeId: null,
            submit: false
        });
    }

    handleDeleteSetting = () => {
        this.setState({
            showDeleteModal: true
        });
    }

    handleCancel = () => {
        this.clear();
    }

    handleOnCloseModal = () => {
        this.setState({
            showDeleteModal: false
        });
    }

    handleDeleteFlowSetting = () => {
        axios({
            url: resolveClientUrl('EmployeeFlowSetting/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeId: this.state.selectedEmployeeId
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
                this.clear();
                this.getEmployeeHaveSetting();
                this.getEmployeeNotHaveSetting();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const { isHost } = this.props;

        let employeeOptions = this.state.member ?
            this.props.employeeOptions.filter(e => e.value !== this.state.member.employeeId) : this.props.employeeOptions;

        return (
            <div className="row employee-flow">
                <ConfiguredEmployeeList
                    employees={this.state.configuredEmployees}
                    selectedEmployeeId={this.state.selectedEmployeeId}
                    onClick={this.handleSelectEmployee}
                />
                <div className="flow-settings">
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th className="member">{getResource('Member')}</th>
                                <th className="evaluator">{getResource('Evaluator')}</th>
                                <th className="reviewer">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td className="col-self">
                                    {isHost &&
                                        <Select className={`react-select-container ${this.state.submit && !this.state.member && 'input-validation-error'}`}
                                            classNamePrefix="react-select"
                                            value={null} options={this.state.unconfiguredEmployeeOptions}
                                            isClearable={false} placeholder="Nickname or full name"
                                            onChange={this.handleMemberChange}
                                            isDisabled={this.state.selectedEmployeeId !== null}
                                        />
                                    }
                                    {this.state.member &&
                                        <div className="flow-item">
                                            <img className="avatar" src={this.state.member.avatar} />
                                            <div className="personal-info">
                                                <span className="txt-bold">{this.state.member.nickName}</span>
                                                <p>{this.state.member.fullName}</p>
                                            </div>
                                        </div>
                                    }
                                </td>
                                <td className="col-evaluator">
                                    {isHost &&
                                        <Select className={`react-select-container ${this.state.submit && !this.state.evaluator && 'input-validation-error'}`}
                                            classNamePrefix="react-select"
                                            value={null} options={employeeOptions}
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
                                    {isHost &&
                                        <Select className={`react-select-container ${this.state.submit && !this.state.reviewer && 'input-validation-error'}`}
                                            classNamePrefix="react-select"
                                            value={null} options={employeeOptions}
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
                        isHost &&
                        <div className="button-group" >
                            <Button type="button" disabled={this.state.selectedEmployeeId === null} className="btn btn-cancel" onClick={this.handleDeleteSetting}>{getResource('Delete')}</Button>
                            <Button type="button" disabled={!this.state.member && !this.state.evaluator && !this.state.reviewer}
                                className="btn btn-cancel" onClick={this.handleCancel}
                            >
                                {getResource('Cancel')}
                            </Button>
                            <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}
                                disabled={!this.state.member && !this.state.evaluator && !this.state.reviewer}
                            >
                                {getResource('Submit')}
                            </Button>
                        </div>
                    }
                </div>
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this employee's setting"
                        showModal={this.state.showDeleteModal}
                        onCloseModal={this.handleOnCloseModal}
                        onSubmit={this.handleDeleteFlowSetting}
                    />
                }

            </div>
        );
    }
}