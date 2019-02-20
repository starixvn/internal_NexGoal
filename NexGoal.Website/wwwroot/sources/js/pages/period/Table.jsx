import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';

import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ConfirmationModal from '../../shared/components/ConfirmationModal';
import CreatePeriod from './CreatePeriod';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            periodSelected: null,
            showCreateModal: false,
            showDeleteModal: false
        };
    }

    columns = [
        {
            dataField: 'rowNum',
            isDummyField: true,
            text: `${getResource('SymbolCount')}`,
            classes: 'text-center',
            headerStyle: { width: '50px' },
            formatter: (cell, row, rowIndex) => {
                return <Fragment>{rowIndex + 1}</Fragment>;
            }
        },
        {
            dataField: 'name',
            text: `${getResource('Name')}`
        },
        {
            dataField: 'startDate',
            text: `${getResource('StartDate')}`,
            classes: 'text-center',
            headerStyle: { width: '120px' },
            formatter: (cell, row) => {
                return row.startDate ? moment(row.startDate).format('DD MMM, YYYY') : '';
            }
        },
        {
            dataField: 'endDate',
            text: `${getResource('EndDate')}`,
            classes: 'text-center',
            headerStyle: { width: '120px' },
            formatter: (cell, row) => {
                return row.endDate ? moment(row.endDate).format('DD MMM, YYYY') : '';
            }
        },
        {
            dataField: 'totalEmployees',
            text: 'Total Employees',
            classes: 'text-center'
        },
        {
            dataField: 'totalUnfinished',
            text: 'Unfinished PA',
            classes: 'text-center'
        },
        {
            dataField: 'isActive',
            text: `${getResource('Active')}`,
            classes: 'text-center',
            headerStyle: { width: '80px' },
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.isHost) {
                    return <div className="checkbox"><input id={'chb-active-' + rowIndex} type='checkbox' defaultChecked={row.isActive} onChange={(e) => this.editPeriodStatus(e, row)} className="checkbox-custom"/><label htmlFor={'chb-active-' + rowIndex} /></div>;
                }

                return <div className="checkbox"><input type='checkbox' id={'chb-active-' + rowIndex} checked={row.isActive} readOnly className="checkbox-custom"/><label htmlFor={'chb-active-' + rowIndex} /></div>;
            }
        },
        {
            dataField: '',
            text: `${getResource('Action')}`,
            classes: 'text-center',
            headerStyle: { width: '120px' },
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.isHost) {
                    return (
                        <Fragment>
                            <a className="fa fa-pencil-square-o" title={getResource('Edit')} href={resolveClientUrl(`period/edit/${row.periodId}`)} />
                            <a className="fa fa-trash-o remove" title={getResource('Delete')} onClick={() => this.handleShowDeleteModal(row)} />
                        </Fragment>
                    );
                }

                return '';
            }
        }];

    editPeriodStatus = (event, period) => {
        var isActive = event.target.checked;

        axios({
            url: resolveClientUrl('period/editstatus'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: period.periodId,
                isActive: isActive
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
                this.props.onUpdateStatus(period.periodId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleShowCreateModal = () => this.setState({ showCreateModal: true })

    handleCloseCreateModal = () => this.setState({ showCreateModal: false })

    handleShowDeleteModal = period => this.setState({ showDeleteModal: true, periodSelected: period })

    handleCloseDeleteModal = () => this.setState({ showDeleteModal: false, periodSelected: null })

    handleDeletePeriod = () => {
        axios({
            url: resolveClientUrl(`period/delete`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.state.periodSelected.periodId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('DeletedSuccessfully'));

                this.props.onDeletePeriod(this.state.periodSelected.periodId);
                this.handleCloseDeleteModal();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        return (
            <Fragment>
                <ToolkitProvider keyField="periodId" data={this.props.periods} columns={this.columns} search >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <div className="search-wrapper clearfix">
                                        <div className="pull-left">
                                            {
                                                this.props.isHost &&
                                                <button className="btn btn-primary" onClick={this.handleShowCreateModal}>
                                                    {getResource('AddEvaluationPeriod')}
                                                </button>
                                            }
                                        </div>
                                        <div className="search pull-right">
                                            <Search.SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                            <span className="fa fa-search icon" />
                                        </div>
                                    </div>
                                    <BootstrapTable {...props.baseProps} noDataIndication={noDataIndication} striped />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
                {
                    this.state.showCreateModal &&
                    <CreatePeriod showModal={this.state.showCreateModal}
                        closeModal={this.handleCloseCreateModal}
                        onCreateSuccess={this.props.refreshData}
                    />
                }
                {
                    this.state.showDeleteModal &&
                    <ConfirmationModal
                        action="delete"
                        name={this.state.periodSelected.name}
                        showModal={this.state.showDeleteModal}
                        onCloseModal={this.handleCloseDeleteModal}
                        onSubmit={this.handleDeletePeriod}
                    />
                }
            </Fragment>
        );
    }
}