import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';
import CreatePeriod from './CreatePeriod';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showModal: false
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
                    return (<input type='checkbox' defaultChecked={row.isActive} onChange={(e) => this.editDisciplineStatus(e, row)} />);
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
                            <a className="fa fa-trash-o text-danger" title="Delete" onClick={() => this.props.editDiscipline(row)}></a>
                        </Fragment>
                    );
                }

                return '';
            }
        }];

    showModal = () => this.setState({ showModal: true })

    closeModal = () => this.setState({ showModal: false })

    render() {
        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left">
                        {this.props.hasHostPermission &&
                            <button className="btn btn-primary" onClick={this.showModal}><i className="fa fa-plus"></i> Create Evaluation Period</button>
                        }
                    </div>
                    <div className="search pull-right">
                        <input className="search-textbox" ref={this.searchRef} onChange={this.handleTableFilterChange} />
                        <span className="fa fa-search icon"></span>
                    </div>
                </div>
                <BootstrapTable noDataIndication={this.noDataDisplay} ref={this.tableRef} keyField='periodId' data={this.props.data} columns={this.columns} />
                <CreatePeriod showModal={this.state.showModal} closeModal={this.closeModal} targetTypes={this.props.targetTypes} onCreateSuccess={this.props.updateData} />
            </Fragment>
        );
    }
}