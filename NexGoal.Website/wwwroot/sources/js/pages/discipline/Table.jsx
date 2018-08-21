import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import moment from 'moment';
import axios from 'axios';
import { stringify } from 'qs';
import { Form } from 'formik';
import { toast } from 'react-toastify';

import { PaginationOptions } from '../../constants';
import commonFunctions from '../../common-functions';

const { SearchBar } = Search;

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    editDisciplineStatus = (e, discipline) => {
        var isActive = e.target.checked;

        axios({
            url: commonFunctions.getRootUrl() + 'discipline/editstatus',
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: discipline.disciplineId,
                isActive: isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            var data = response.data;

            if (data.isSuccessful) {
                toast.success('Update Successfully');
                this.props.onUpdateStatus(discipline.disciplineId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);;
    }

    editDiscipline = (discipline) => {
    }

    render() {
        const columns = [
            {
                dataField: 'rowNum',
                text: '#',
                classes: 'text-center'
            },
            {
                dataField: 'nickName',
                text: 'Nick Name'
            },
            {
                dataField: 'fullName',
                text: 'Full Name'
            },
            {
                dataField: 'group',
                text: 'Team',
            },
            {
                dataField: 'description',
                text: 'Content'
            },
            {
                dataField: 'effectiveDate',
                text: 'Issue Date',
                formatter: (cell, row, rowIndex, colIndex) => {
                    return `${moment(row.effectiveDate).format('DD MMM, YYYY')}`;
                }
            },
            {
                dataField: 'creatorFullName',
                text: 'Creator',
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
                    if (this.props.isHost) {
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
                    if (this.props.isHost) {
                        return (
                            <a className="fa fa-pencil-square-o" title="Edit" onClick={() => this.props.editDiscipline(row)}></a>
                        );
                    }

                    return '';
                }
            }];

        return (
            <ToolkitProvider keyField="disciplineId" data={this.props.data} noDataIndication={this.noDataDisplay} columns={columns} search >
                {
                    props => {
                        return (
                            <Fragment>
                                <div className="search-wrapper clearfix">
                                    <div className="pull-left">
                                        {this.props.isHost &&
                                            <button className="btn btn-primary" onClick={this.props.addDiscipline}>
                                                <i className="fa fa-plus"></i> Add Discipline
                                            </button>
                                        }
                                    </div>
                                    <div className="search pull-right">
                                        <SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                        <span className="fa fa-search icon"></span>
                                    </div>
                                </div>
                                <BootstrapTable {...props.baseProps} pagination={paginationFactory(PaginationOptions)} striped />
                            </Fragment>
                        );
                    }
                }
            </ToolkitProvider>
        );
    }
}