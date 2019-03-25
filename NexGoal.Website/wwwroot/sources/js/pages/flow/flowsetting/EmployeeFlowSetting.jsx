import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PerformanceProcess } from '../../../constants';
import EmployeeFlowFilter from './EmployeeFlowFilter';
import EvaluatorList from './EvaluatorList';
import Sortable from './Sortable';

class EmployeeFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            evaluators: [],
            reviewers: [],
            evaluatorSelected: null,
            employeeSelected: null,
            showDeleteButton: false,
            showDeleteModal: false
        };
    }

    getFlowSetting = employeeId => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetById?employeeId=${employeeId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    evaluators: data.evaluator !== null ? [data.evaluator] : [],
                    reviewers: data.reviewer !== null ? [data.reviewer] : [],
                    employeeSelected: {
                        employeeId: data.employeeId,
                        code: data.code,
                        nickName: data.nickName,
                        fullName: data.fullName,
                        avatar: data.avatar
                    },
                    showDeleteButton: data.evaluator !== null || data.reviewer !== null
                });
            }).catch(axiosError);
    }

    handleOnEmployeeSelected = employeeId => {
        this.getFlowSetting(employeeId);
    }

    handleAddItem = (toIndex, employeeId, sender) => {
        let items = this.state[sender].slice();
        let processId;

        switch (sender) {
            case 'evaluators':
                processId = PerformanceProcess.Evaluator;
                break;

            case 'reviewers':
                processId = PerformanceProcess.Reviewer;
                break;
        }

        const index = items.findIndex(item => item.evaluatorId === employeeId);
        if (index < 0) {
            if ((sender === 'evaluators' || sender === 'reviewers') && items.length > 0) {
                toast.error('Evaluator or Reviewer is single setting!');
            } else {
                axios(resolveClientUrl(`Flow/GetEmployeeById?employeeId=${employeeId}`))
                    .then(response => {
                        const data = response.data;
                        const item = {
                            processId: processId,
                            evaluatorId: data.employeeId,
                            nickName: data.nickName,
                            fullName: data.fullName,
                            code: data.code,
                            avatar: data.avatar
                        };

                        items.splice(toIndex, 0, item);
                        this.setState({ [sender]: items });
                    }).catch(axiosError);
            }
        } else {
            toast.error('This employee has already existed!');
        }
    }

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
        if (this.state.evaluators.length === 0
            || this.state.reviewers.length === 0) {
            toast.error('Please setting flow for Evaluator and Reviewer.');
        } else {
            let settings = [];

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
                url: resolveClientUrl('EmployeeFlowSetting/Edit'),
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    employeeId: this.state.employeeSelected.employeeId,
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

                    this.setState({ showDeleteButton: true });
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
            url: resolveClientUrl('EmployeeFlowSetting/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeId: this.state.employeeSelected.employeeId
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

                this.setState({
                    evaluators: [],
                    reviewers: [],
                    showDeleteButton: false
                });
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const employeeSelected = this.state.employeeSelected;
        const { isHost, groupOptions } = this.props;

        return (
            <div className="employee-flow">
                <EmployeeFlowFilter
                    isHost={isHost}
                    hasSetting={this.state.showDeleteButton}
                    groupOptions={groupOptions}
                    onEmployeeSelected={this.handleOnEmployeeSelected}
                />
                <div className="boxtitle">PA Processing</div>
                <div className="row">
                    <div className="col-sm-3">
                        <EvaluatorList groupOptions={groupOptions} isHost={isHost} />
                    </div>
                    <div className="col-sm-9 flow-settings">
                        <table className="table table-bordered">
                            <thead>
                                <tr>
                                    <th>{getResource('Self')}</th>
                                    <th>{getResource('Evaluator')}</th>
                                    <th>{getResource('Reviewer')}</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td className="col-self">
                                        {
                                            employeeSelected &&
                                            <ul>
                                                <li className="alert alert-info">
                                                    <img src={employeeSelected.avatar} />
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
                        {
                            isHost && employeeSelected &&
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
                            name="this employee's setting"
                            showModal={this.state.showDeleteModal}
                            onCloseModal={this.handleOnCloseModal}
                            onSubmit={this.handleDeleteFlowSetting}
                        />
                    }
                </div>
            </div>
        );
    }
}

export default EmployeeFlowSetting;