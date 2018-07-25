import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import BootstrapTable from 'react-bootstrap-table-next';

export default class Table extends Component {
    constructor(props) {
        super(props);

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
            dataField: 'type',
            text: 'Type'
        },
        {
            dataField: 'startDate',
            text: 'Start Date',
        },
        {
            dataField: 'endDate',
            text: 'End Date'
        },
        {
            dataField: '123',
            text: 'Total Employees',
        },
        {
            dataField: '456',
            text: 'Unfinished PA',
            classes: 'text-center',
            formatter: (cell, row, rowIndex, colIndex) => {
                return `${row.creatorFullName} (${row.creatorNickName})`;
            }
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
                            <a className="fa fa-trash" title="Delete" onClick={() => this.props.editDiscipline(row)}></a>
                        </Fragment>
                    );
                }

                return '';
            }
        }];

    render() {
        return (
            <Fragment>
                <div className="search-wrapper clearfix" >
                    <div className="pull-left">
                        {this.props.hasHostPermission &&
                            <button className="btn btn-primary" onClick={this.addPeriod}><i className="fa fa-plus"></i> Add Evaluation Period</button>
                        }
                    </div>
                    <div className="search pull-right">
                        <input className="search-textbox" ref={this.searchRef} onChange={this.handleTableFilterChange} />
                        <span className="fa fa-search icon"></span>
                    </div>
                </div>
                <BootstrapTable noDataIndication={this.noDataDisplay} ref={this.tableRef} keyField='periodId' data={this.props.data} columns={this.columns} />
            </Fragment>
        );
    }
}