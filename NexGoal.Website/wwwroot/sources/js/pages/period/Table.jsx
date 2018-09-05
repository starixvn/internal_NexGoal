import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import CreatePeriod from './CreatePeriod';
import DeletePeriod from './DeletePeriod';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false
        };

        this.tableRef = React.createRef();
    }

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    columns = [
        {
            dataField: 'rowNum',
            text: '#',
            classes: 'text-center'
        },
        {
            dataField: 'name',
            text: 'Name'
        },
        {
            dataField: 'targetType',
            text: 'Type'
        },
        {
            dataField: 'startDate',
            text: 'Start Date',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                return row.startDate ? moment(row.startDate).format('DD MMM, YYYY') : '';
            }
        },
        {
            dataField: 'endDate',
            text: 'End Date',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                return row.endDate ? moment(row.endDate).format('DD MMM, YYYY') : '';
            }
        },
        {
            dataField: '123',
            text: 'Total Employees',
        },
        {
            dataField: '456',
            text: 'Unfinished PA',
            classes: 'text-center',
        },
        {
            dataField: 'isActive',
            text: 'Active',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.hasHostPermission) {
                    return (<input type='checkbox' defaultChecked={row.isActive} onChange={(e) => this.editPeriodStatus(e, row)} />);
                }

                return (<input type='checkbox' checked={row.isActive} readOnly />);
            }
        },
        {
            dataField: '',
            text: 'Action',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.hasHostPermission) {
                    return (
                        <Fragment>
                            <a className="fa fa-pencil-square-o" title="Edit" href={`/period/edit/${row.periodId}`} />
                            <a className="fa fa-trash-o remove" title="Delete" onClick={() => this.showDeleteModal(row)} />
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
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
                this.props.onUpdateStatus(period.periodId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    showCreateModal = () => this.setState({ showCreateModal: true })

    closeCreateModal = () => this.setState({ showCreateModal: false })

    showDeleteModal = period => this.setState({ showDeleteModal: true, deletedPeriod: period })

    closeDeleteModal = () => this.setState({ showDeleteModal: false })

    render() {
        return (
            <Fragment>
                <ToolkitProvider keyField="periodId" data={this.props.data} noDataIndication={this.noDataDisplay} columns={this.columns} search >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <div className="search-wrapper clearfix">
                                        <div className="pull-left">
                                            {this.props.hasHostPermission &&
                                                <button className="btn btn-primary" onClick={this.showCreateModal}><i className="fa fa-plus" /> Create Evaluation Period</button>
                                            }
                                        </div>
                                        <div className="search pull-right">
                                            <Search.SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                            <span className="fa fa-search icon" />
                                        </div>
                                    </div>
                                    <BootstrapTable {...props.baseProps} striped />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
                <CreatePeriod showModal={this.state.showCreateModal} closeModal={this.closeCreateModal} targetTypes={this.props.targetTypes} onCreateSuccess={this.props.updateData} />
                <DeletePeriod showModal={this.state.showDeleteModal} closeModal={this.closeDeleteModal} period={this.state.deletedPeriod} onDeleteSuccess={this.props.onDeletePeriod} />
            </Fragment>
        );
    }
}