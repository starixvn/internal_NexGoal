import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import commonFunctions from '../../common-functions';
import Row from './Row';
import Create from './Create';
import Delete from './Delete';
import EditStatus from './EditStatus';

function THead(props) {
    return (
        <thead>{props.children}</thead>
    );
}

function TBody(props) {
    return (
        <tbody>{props.children}</tbody>
    );
}

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

    handleCreateClick() {
        this.setState({ showCreateModal: true });
    }

    closeCreateModal() {
        this.setState({ showCreateModal: false });
    }

    handleDeleteClick(flowTemplateSelected) {
        this.setState({
            showDeleteModal: true,
            flowTemplateSelected: flowTemplateSelected
        });
    }

    closeDeleteModal() {
        this.setState({
            showDeleteModal: false,
            flowTemplateSelected: null
        });
    }

    handleDeleteFlowTemplate() {
        axios({
            url: commonFunctions.getRootUrl() + 'Flow/DeleteFlowTemplate',
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
                toast.success('Deleted successfully!');

                this.closeDeleteModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    handleCheckboxChange(flowTemplateSelected) {
        this.setState({
            showEditStatusModal: true,
            flowTemplateSelected: flowTemplateSelected
        });
    }

    handleEditStatusFlowTemplate() {
        axios({
            url: commonFunctions.getRootUrl() + 'Flow/EditStatusFlowTemplate',
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
                toast.success('Updated successfully!');

                this.closeEditStatusModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    closeEditStatusModal() {
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
                    <THead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.handleCreateClick.bind(this)}>
                                        <i className="fa fa-plus"></i>Create Evaluation Flow
                                    </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Type</th>
                            <th>Active</th>
                            <th>Description</th>
                            <th>Action</th>
                        </tr>
                    </THead>
                    <TBody>
                        {flowTemplates.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="6">No information available!</td>
                            </tr>
                        }
                        {flowTemplates.length > 0 &&
                            flowTemplates.map((flowTemplate, index) => {
                                return (
                                    <Row key={'flowTemplate-' + flowTemplate.flowTemplateId}
                                        rowIndex={index + 1}
                                        flowTemplate={flowTemplate}
                                        isHost={isHost}
                                        handleDeleteClick={this.handleDeleteClick.bind(this)}
                                        handleCheckboxChange={this.handleCheckboxChange.bind(this)} />)
                            })
                        }
                    </TBody>
                </table>

                {
                    this.state.showCreateModal &&
                    <Create
                        showModal={this.state.showCreateModal}
                        closeModal={this.closeCreateModal.bind(this)}
                        targetTypes={this.props.targetTypes}
                        handleFilterSubmit={this.props.handleFilterSubmit} />
                }
                {
                    this.state.showDeleteModal &&
                    <Delete
                        name={this.state.flowTemplateSelected.name}
                        showModal={this.state.showDeleteModal}
                        closeModal={this.closeDeleteModal.bind(this)}
                        handleDeleteFlowTemplate={this.handleDeleteFlowTemplate.bind(this)} />
                }
                {
                    this.state.showEditStatusModal &&
                    <EditStatus
                        action={this.state.flowTemplateSelected.isActive ? 'activate' : 'deactivate'}
                        name={this.state.flowTemplateSelected.name}
                        showModal={this.state.showEditStatusModal}
                        closeModal={this.closeEditStatusModal.bind(this)}
                        handleEditStatusFlowTemplate={this.handleEditStatusFlowTemplate.bind(this)} />
                }
            </Fragment>
        );
    }
}

export default Table;