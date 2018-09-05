import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';

import Row from './Row';
import CreateEOY from './Create';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            eoySelected: null
        };
    }

    showCreateModal() {
        this.setState({ showCreateModal: true });
    }

    closeCreateModal() {
        this.setState({ showCreateModal: false });
    }

    showDeleteModal(eoySelected) {
        this.setState({
            showDeleteModal: true,
            eoySelected: eoySelected
        });
    }

    closeDeleteModal() {
        this.setState({
            showDeleteModal: false,
            eoySelected: null
        });
    }

    handleDeleteEOY() {
        axios({
            url: resolveClientUrl('EOY/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: this.state.eoySelected.eoqyId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.closeDeleteModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const isHost = this.props.isHost;
        const eoys = this.props.eoys;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <thead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.showCreateModal.bind(this)}>
                                        <i className="fa fa-plus" />Create Nomination Period
                                </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>#</th>
                            <th>{getResource('Year')}</th>
                            <th>{getResource('StartDateForNominator')}</th>
                            <th>{getResource('EndDateForNominator')}</th>
                            <th>{getResource('RemindDateForNominator')}</th>
                            <th>{getResource('EndDateForCommittee')}</th>
                            <th>{getResource('Status')}</th>
                            <th>{getResource('Action')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {eoys.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">{getResource('NoInformationAvailable')}</td>
                            </tr>
                        }
                        {eoys.length > 0 &&
                            eoys.map((eoy, index) => {
                                return (
                                    <Row key={'eoy-' + eoy.eoqyId}
                                        rowIndex={index + 1}
                                        eoy={eoy}
                                        isHost={isHost}
                                        showDeleteModal={this.showDeleteModal.bind(this)}
                                    />
                                );
                            })
                        }
                    </tbody>
                </table>

                {
                    this.state.showCreateModal &&
                    <CreateEOY
                        showModal={this.state.showCreateModal}
                        closeModal={this.closeCreateModal.bind(this)}
                        handleFilterSubmit={this.props.handleFilterSubmit}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name={this.state.eoySelected.name}
                        showModal={this.state.showDeleteModal}
                        closeModal={this.closeDeleteModal.bind(this)}
                        handleSubmit={this.handleDeleteEOY.bind(this)}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;