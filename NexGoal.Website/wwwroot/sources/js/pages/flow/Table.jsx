import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import Row from './Row';
import Create from './Create';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            showEditStatusModal: false,
            flowTemplateSelected: null
        };
    }

    handleOnCreateClick = () => this.setState({ showCreateModal: true });

    handleOnCloseCreateModal = () => this.setState({ showCreateModal: false });

    handleOnDeleteClick = (flowTemplateSelected) => {
        this.setState({
            showDeleteModal: true,
            flowTemplateSelected: flowTemplateSelected
        });
    }

    handleOnCloseDeleteModal = () => {
        this.setState({
            showDeleteModal: false,
            flowTemplateSelected: null
        });
    }

    handleDeleteFlowTemplate = () => {
        axios({
            url: resolveClientUrl('Flow/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                flowTemplateId: this.state.flowTemplateSelected.flowTemplateId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.handleOnCloseDeleteModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleOnCheckboxChange = (flowTemplateSelected) => {
        this.setState({
            showEditStatusModal: true,
            flowTemplateSelected: flowTemplateSelected
        });
    }

    handleEditStatusFlowTemplate = () => {
        axios({
            url: resolveClientUrl('Flow/EditStatus'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                flowTemplateId: this.state.flowTemplateSelected.flowTemplateId,
                isActive: this.state.flowTemplateSelected.isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));

                this.handleOnCloseEditStatusModal();
                this.props.onFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleOnCloseEditStatusModal = () => {
        this.setState({
            showEditStatusModal: false,
            flowTemplateSelected: null
        });
    }

    render() {
        const isHost = this.props.isHost;
        const flowTemplates = this.props.flowTemplates;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <thead>
                        {
                            isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.handleOnCreateClick}>
                                        <i className="fa fa-plus" />{getResource('AddEvaluationFlow')}
                                    </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>{getResource('SymbolCount')}</th>
                            <th>{getResource('Name')}</th>
                            <th>{getResource('TargetType')}</th>
                            <th>{getResource('Active')}</th>
                            <th>{getResource('Description')}</th>
                            <th>{getResource('Action')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            flowTemplates.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="6">{getResource('NoRecordFound')}</td>
                            </tr>
                        }
                        {
                            flowTemplates.length > 0 &&
                            flowTemplates.map((flowTemplate, index) => {
                                return (
                                    <Row key={'flowTemplate-' + flowTemplate.flowTemplateId}
                                        rowIndex={index + 1}
                                        flowTemplate={flowTemplate}
                                        isHost={isHost}
                                        onDelete={this.handleOnDeleteClick}
                                        onCheckboxChange={this.handleOnCheckboxChange}
                                    />
                                );
                            })
                        }
                    </tbody>
                </table>

                {
                    this.state.showCreateModal &&
                    <Create
                        showModal={this.state.showCreateModal}
                        closeModal={this.handleOnCloseCreateModal}
                        targetTypes={this.props.targetTypes}
                        onFilterSubmit={this.props.onFilterSubmit}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name={this.state.flowTemplateSelected.name}
                        showModal={this.state.showDeleteModal}
                        onCloseModal={this.handleOnCloseDeleteModal}
                        onSubmit={this.handleDeleteFlowTemplate}
                    />
                }
                {
                    this.state.showEditStatusModal &&
                    <ConfirmationModal
                        action={this.state.flowTemplateSelected.isActive ? 'activate' : 'deactivate'}
                        name={this.state.flowTemplateSelected.name}
                        showModal={this.state.showEditStatusModal}
                        onCloseModal={this.handleOnCloseEditStatusModal}
                        onSubmit={this.handleEditStatusFlowTemplate}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;