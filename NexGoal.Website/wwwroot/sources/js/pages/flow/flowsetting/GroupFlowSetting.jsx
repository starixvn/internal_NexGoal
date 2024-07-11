import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import { PostRequestHeaders } from '../../../constants';
import GroupList from './GroupList';
import FlowSettingTable from './FlowSettingTable';
import { handleEvaluatorChange, handleReviewerChange } from '../common-functions';

class GroupFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSettings: [],
            evaluator: null,
            reviewer: null,
            selectedGroup: null,
            enableDeleteButton: false,
            showDeleteModal: false,
            isSubmit: false
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('GroupFlowSetting/GetGroups'))
        ];

        Promise.all(promises)
            .then(([groupResponse]) => {
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
                    evaluator: !data.evaluator ? data.evaluator : {
                        ...data.evaluator,
                        avatar: `${document.imageUrl}/${data.evaluator.code}.jpg`
                    },
                    reviewer: !data.reviewer ? data.reviewer : {
                        ...data.reviewer,
                        avatar: `${document.imageUrl}/${data.reviewer.code}.jpg`
                    },
                    enableDeleteButton: data.evaluator !== null || data.reviewer !== null
                });
            }).catch(axiosError);
    };

    handleSelectGroup = selectedGroup => {
        this.setState({
            selectedGroup: selectedGroup
        });

        this.getFlowSetting(selectedGroup.groupId);
    };

    handleEvaluatorChange = selectedEvaluator => {
        handleEvaluatorChange(this, selectedEvaluator.value);
    };

    handleReviewerChange = selectedReviewer => {
        handleReviewerChange(this, selectedReviewer.value);
    };

    handleSaveFlowSetting = () => {
        if (!this.state.isSubmit) {
            this.setState({ isSubmit: true });
        }

        const errorFields = [];
        if (!this.state.evaluator) {
            errorFields.push('Evaluator');
        }

        if (!this.state.reviewer) {
            errorFields.push('Reviewer');
        }

        if (errorFields.length > 0) {
            toast.error(`Please select ${errorFields.join(', ')}`);
        } else {
            const settings = [{
                processId: this.state.evaluator.processId,
                evaluatorId: this.state.evaluator.evaluatorId,
                priority: 0
            }, {
                processId: this.state.reviewer.processId,
                evaluatorId: this.state.reviewer.evaluatorId,
                priority: 0
            }];

            axios({
                url: resolveClientUrl('GroupFlowSetting/Edit'),
                method: 'POST',
                data: stringify({
                    __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                    groupId: this.state.selectedGroup.groupId,
                    settings: settings
                }),
                headers: PostRequestHeaders
            }).then(response => {
                const data = response.data;
                this.setState({ isSubmit: false });
                if (data.isSuccessful) {
                    toast.success(getResource('UpdatedSuccessfully'));

                    const groupSettings = this.state.groupSettings.slice();
                    const selectedGroup = groupSettings.find(g => g.groupId === this.state.selectedGroup.groupId);

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
    };

    handleOnDeleteClick = () => {
        this.setState({
            showDeleteModal: true
        });
    };

    handleOnCloseModal = () => {
        this.setState({
            showDeleteModal: false
        });
    };

    handleDeleteFlowSetting = () => {
        axios({
            url: resolveClientUrl('GroupFlowSetting/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                groupId: this.state.selectedGroup.groupId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.handleOnCloseModal();

                const groupSettings = this.state.groupSettings.slice();
                const selectedGroup = groupSettings.find(g => g.groupId === this.state.selectedGroup.groupId);
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
    };

    renderSelf = () => {
        return (
            <div className="col-self">
                {this.state.selectedGroup &&
                    <div className="flow-item">
                        <i className="avatar icon-draft-member" />
                        <div className="personal-info">
                            <span className="txt-bold">{this.state.selectedGroup.name}</span></div>
                    </div>
                }
            </div>
        );
    };

    render() {
        const { isSubmit, evaluator, reviewer } = this.state;
        const { isHost, employeeOptions } = this.props;

        return (
            <div className="row group-flow">
                <GroupList groups={this.state.groupSettings} onClick={this.handleSelectGroup} />
                <div className="flow-settings">
                    <FlowSettingTable isHost={isHost} isSubmit={isSubmit} isGroup
                        evaluator={evaluator} reviewer={reviewer}
                        employeeOptions={employeeOptions}
                        selfColContent={this.renderSelf()}
                        onChangeEvaluator={this.handleEvaluatorChange}
                        onChangeReviewer={this.handleReviewerChange}
                    />
                    {
                        isHost && this.state.selectedGroup &&
                        <div className="button-group" >
                            <Button type="button" disabled={!this.state.enableDeleteButton} className="btn btn-cancel" onClick={this.handleOnDeleteClick}>
                                {getResource('Delete')}
                            </Button>
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