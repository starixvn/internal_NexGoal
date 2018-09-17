import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ConfirmationModal from '../../../shared/components/ConfirmationModal';
import Row from './Row';
import CreateEOQ from './Create';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
            eoqSelected: null
        };
    }

    handleShowCreateModal = () => this.setState({ showCreateModal: true });

    handleCloseCreateModal = () => this.setState({ showCreateModal: false });

    handleShowDeleteModal = (eoqSelected) => {
        this.setState({
            showDeleteModal: true,
            eoqSelected: eoqSelected
        });
    }

    handleCloseDeleteModal = () => {
        this.setState({
            showDeleteModal: false,
            eoqSelected: null
        });
    }

    handleDeleteEOQ = () => {
        axios({
            url: resolveClientUrl('EOQ/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: this.state.eoqSelected.eoqyId
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
        const eoqs = this.props.eoqs;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <thead>
                        {isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.handleShowCreateModal}>
                                        <i className="fa fa-plus" />{getResource('CreateNominationPeriod')}
                                    </a>
                                </th>
                            </tr>
                        }
                        <tr>
                            <th>{getResource('SymbolCount')}</th>
                            <th>{getResource('Quarter')}</th>
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
                        {eoqs.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">{getResource('NoInformationAvailable')}</td>
                            </tr>
                        }
                        {eoqs.length > 0 &&
                            eoqs.map((eoq, index) => {
                                return (
                                    <Row key={'eoq-' + eoq.eoqyId}
                                        rowIndex={index + 1}
                                        eoq={eoq}
                                        isHost={isHost}
                                        onShowDeleteModal={this.handleShowDeleteModal}
                                    />
                                );
                            })
                        }
                    </tbody>
                </table>

                {
                    this.state.showCreateModal &&
                    <CreateEOQ
                        showModal={this.state.showCreateModal}
                        closeModal={this.handleCloseCreateModal}
                        handleFilterSubmit={this.props.handleFilterSubmit}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name={this.state.eoqSelected.title}
                        showModal={this.state.showDeleteModal}
                        closeModal={this.handleCloseDeleteModal}
                        handleSubmit={this.handleDeleteEOQ}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;