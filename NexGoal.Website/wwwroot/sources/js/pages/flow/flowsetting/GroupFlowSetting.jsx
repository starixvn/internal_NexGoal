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
import EvaluatorList from './EvaluatorList';
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
            showDeleteButton: false,
            showDeleteModal: false
        };

        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleOnReceive = this.handleOnReceive.bind(this);
        this.handleOnDeactivate = this.handleOnDeactivate.bind(this);
        this.handleOnRemoveItem = this.handleOnRemoveItem.bind(this);
        this.handleOnDeleteClick = this.handleOnDeleteClick.bind(this);
        this.handleSaveFlowSetting = this.handleSaveFlowSetting.bind(this);
        this.handleDeleteFlowSetting = this.handleDeleteFlowSetting.bind(this);
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl(`GroupFlowSetting/GetGroupHaveSetting?flowTemplateId=${this.props.flowTemplateId}`))
        ];

        Promise.all(promises)
            .then(([groupResponse, groupSettingResponse]) => {
                this.setState({
                    groupOptions: groupResponse.data,
                    groupSettings: groupSettingResponse.data
                });
            }).catch(axiosError);
    }

    getFlowSetting(groupId) {
        axios(resolveClientUrl(`GroupFlowSetting/GetById?groupId=${groupId}&flowTemplateId=${this.props.flowTemplateId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    humanResources: data.humanResources,
                    evaluators: data.evaluator !== null ? [data.evaluator] : [],
                    reviewers: data.reviewer !== null ? [data.reviewer] : [],
                    showDeleteButton: data.humanResources.length > 0 || data.evaluator !== null || data.reviewer !== null
                });
            }).catch(axiosError);
    }

    handleGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption
        });

        this.getFlowSetting(selectedOption.value);
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
                axios(resolveClientUrl(`Flow/GetEmployeeById?employeeId=${employeeId}`))
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
                    }).catch(axiosError);
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
                url: resolveClientUrl('GroupFlowSetting/Edit'),
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    flowTemplateId: this.props.flowTemplateId,
                    groupId: this.state.groupSelected.value,
                    settings: settings
                }),
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('UpdatedSuccessfully'));

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
            }).catch(axiosError);
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
            url: resolveClientUrl('GroupFlowSetting/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                flowTemplateId: this.props.flowTemplateId,
                groupId: this.state.groupSelected.value
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.handleOnCloseModal();

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
        }).catch(axiosError);
    }

    render() {
        const isHost = this.props.isHost;

        let groupOptions = this.state.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const groupSetting = this.state.groupSettings.map((group) => {
            return group.name;
        }).join(', ');

        return (
            <div className="row group-flow">
                <div className="col-sm-3">
                    <EvaluatorList groupOptions={this.state.groupOptions} isHost={isHost} />
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
                                <Select className="react-select-container" classNamePrefix="react-select"
                                    value={this.state.groupSelected} options={groupOptions}
                                    isClearable={false}
                                    onChange={this.handleGroupChange}
                                />
                            </div>
                        </FormGroup>
                    </Form>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>{getResource('Admin')}</th>
                                <th>{getResource('Self')}</th>
                                <th>{getResource('Evaluator')}</th>
                                <th>{getResource('Reviewer')}</th>
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
                                    {this.state.groupSelected.value &&
                                        <ul>
                                            <li className="alert alert-info">
                                                <img />
                                                <p>Self</p>
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
                    {isHost && this.state.groupSelected.value &&
                        <Form inline>
                            <Button type="button" bsStyle="primary" onClick={this.handleSaveFlowSetting}>{getResource('Submit')}</Button>
                            {this.state.showDeleteButton &&
                                <Button type="button" bsStyle="primary" onClick={this.handleOnDeleteClick}>{getResource('Delete')}</Button>}
                        </Form>
                    }
                </div>
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name="this group's setting"
                        showModal={this.state.showDeleteModal}
                        closeModal={this.handleOnCloseModal.bind(this)}
                        handleSubmit={this.handleDeleteFlowSetting}
                    />
                }
            </div>
        );
    }
}

export default GroupFlowSetting;