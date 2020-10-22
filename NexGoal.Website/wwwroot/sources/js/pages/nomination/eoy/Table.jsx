import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PostRequestHeaders } from '../../../constants';
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

    handleShowCreateEoYModal = () => this.setState({ showCreateModal: true });

    handleCloseCreateModal = () => this.setState({ showCreateModal: false });

    handleShowDeleteModal = eoySelected => {
        this.setState({
            showDeleteModal: true,
            eoySelected: eoySelected
        });
    }

    handleCloseDeleteModal = () => {
        this.setState({
            showDeleteModal: false,
            eoySelected: null
        });
    }

    handleDeleteEOY = () => {
        axios({
            url: resolveClientUrl('NominationPeriod/Delete'),
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: this.state.eoySelected.eoqyId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.closeDeleteModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    renderEoYTableHeader = () => {
        return (
            <tr>
                <th>{getResource('SymbolCount')}</th>
                <th>{getResource('Year')}</th>
                <th>{getResource('StartDateForNominator')}</th>
                <th>{getResource('EndDateForNominator')}</th>
                <th>{getResource('RemindDateForNominator')}</th>
                <th>{getResource('EndDateForCommittee')}</th>
                <th>{getResource('Status')}</th>
                <th>{getResource('Action')}</th>
            </tr>
        );
    }

    render() {
        const { isHost, eoys } = this.props;

        return (
            <Fragment>
                <table className="table table-bordered">
                    <thead>
                        {
                            isHost &&
                            <tr className="header-control">
                                <th colSpan="9">
                                    <a className="pull-left btn btn-primary" onClick={this.handleShowCreateEoYModal}>
                                        {getResource('AddNominationPeriod')}
                                    </a>
                                </th>
                            </tr>
                        }
                        {
                            this.renderEoYTableHeader()
                        }
                    </thead>
                    <tbody>
                        {
                            eoys.length === 0 &&
                            <tr>
                                <td className="empty-result" colSpan="9">{getResource('NoRecordFound')}</td>
                            </tr>
                        }
                        {
                            eoys.length > 0 &&
                            eoys.map((eoy, index) => {
                                return (
                                    <Row key={'eoy-' + eoy.eoqyId}
                                        rowIndex={index + 1}
                                        eoy={eoy}
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
                    <CreateEOY
                        showModal={this.state.showCreateModal}
                        closeModal={this.handleCloseCreateModal}
                        handleFilterSubmit={this.props.handleFilterSubmit}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name={this.state.eoySelected.title}
                        showModal={this.state.showDeleteModal}
                        onCloseModal={this.handleCloseDeleteModal}
                        onSubmit={this.handleDeleteEOY}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;