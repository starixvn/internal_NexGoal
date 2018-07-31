import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import CreatePeriod from './CreatePeriod';
import DeletePeriod from './DeletePeriod';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showCreateModal: false,
            showDeleteModal: false,
        }

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
                            <a className="fa fa-pencil-square-o" title="Edit" onClick={() => this.props.editDiscipline(row)}></a>
                            <a className="fa fa-trash-o remove" title="Delete" onClick={() => this.showDeleteModal(row)}></a>
                        </Fragment>
                    );
                }

                return '';
            }
        }];

    editPeriodStatus = (event, period) => {
        var isActive = event.target.checked;

        axios({
            url: '/period/editstatus',
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
        }).catch(commonFunctions.axiosError);;
    }

    showCreateModal = () => this.setState({ showCreateModal: true })

    closeCreateModal = () => this.setState({ showCreateModal: false })

    showDeleteModal = period => this.setState({ showDeleteModal: true, deletedPeriod: period })

    closeDeleteModal = () => this.setState({ showDeleteModal: false })

    render() {
        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left">
                        {this.props.hasHostPermission &&
                            <button className="btn btn-primary" onClick={this.showCreateModal}><i className="fa fa-plus"></i> Create Evaluation Period</button>
                        }
                    </div>
                    <div className="search pull-right">
                        <input className="search-textbox" ref={this.searchRef} onChange={this.handleTableFilterChange} />
                        <span className="fa fa-search icon"></span>
                    </div>
                </div>
                <BootstrapTable noDataIndication={this.noDataDisplay} ref={this.tableRef} keyField='periodId' data={this.props.data} columns={this.columns} />
                <CreatePeriod showModal={this.state.showCreateModal} closeModal={this.closeCreateModal} targetTypes={this.props.targetTypes} onCreateSuccess={this.props.updateData} />
                <DeletePeriod showModal={this.state.showDeleteModal} closeModal={this.closeDeleteModal} period={this.state.deletedPeriod} onDeleteSuccess={this.props.onDeletePeriod} />
            </Fragment>
        );
    }
}