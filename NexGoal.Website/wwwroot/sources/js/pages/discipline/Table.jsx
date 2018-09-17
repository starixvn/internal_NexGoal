import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import moment from 'moment';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';

import { PaginationOptions } from '../../constants';
import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

const { SearchBar } = Search;

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    columns = [
        {
            dataField: 'rowNum',
            text: `${getResource('SymbolCount')}`,
            classes: 'text-center',
            headerStyle: { width: '50px' }
        },
        {
            dataField: 'nickName',
            text: `${getResource('NickName')}`,
            headerStyle: { width: '120px' }
        },
        {
            dataField: 'fullName',
            text: `${getResource('FullName')}`
        },
        {
            dataField: 'group',
            text: `${getResource('Team')}`
        },
        {
            dataField: 'description',
            text: `${getResource('Content')}`
        },
        {
            dataField: 'effectiveDate',
            text: `${getResource('IssueDate')}`,
            classes: 'text-center',
            headerStyle: { width: '120px' },
            formatter: (cell, row, rowIndex, colIndex) => {
                return `${moment(row.effectiveDate).format('DD MMM, YYYY')}`;
            }
        },
        {
            dataField: 'creatorFullName',
            text: `${getResource('Creator')}`,
            formatter: (cell, row, rowIndex, colIndex) => {
                return `${row.creatorFullName} (${row.creatorNickName})`;
            }
        },
        {
            dataField: 'isActive',
            text: `${getResource('Active')}`,
            classes: 'text-center',
            headerStyle: { width: '80px' },
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.isHost) {
                    return <input type='checkbox' checked={row.isActive} onChange={this.editDisciplineStatus.bind(this, row)} />;
                }

                return <input type='checkbox' checked={row.isActive} readOnly />;
            }
        },
        {
            dataField: '',
            text: `${getResource('Action')}`,
            classes: 'text-center',
            headerStyle: { width: '80px' },
            isDummyField: true,
            formatter: (cell, row, rowIndex, colIndex) => {
                if (this.props.isHost) {
                    return <a className="fa fa-pencil-square-o" title="Edit" onClick={this.editDiscipline.bind(this, row)} />;
                }

                return '';
            }
        }
    ]

    editDiscipline = discipline => this.props.editDiscipline(discipline)

    editDisciplineStatus = (discipline, e) => {
        var isActive = e.target.checked;

        axios({
            url: resolveClientUrl('discipline/editstatus'),
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
                toast.success(getResource('UpdatedSuccessfully'));
                this.props.onUpdateStatus(discipline.disciplineId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        return (
            <ToolkitProvider keyField="disciplineId" data={this.props.data} columns={this.columns} search >
                {
                    props => {
                        return (
                            <Fragment>
                                <div className="search-wrapper clearfix">
                                    <div className="pull-left">
                                        {this.props.isHost &&
                                            <button className="btn btn-primary" onClick={this.props.addDiscipline}>
                                                <i className="fa fa-plus" /> {getResource('AddDiscipline')}
                                            </button>
                                        }
                                    </div>
                                    <div className="search pull-right">
                                        <SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                        <span className="fa fa-search icon" />
                                    </div>
                                </div>
                                <BootstrapTable {...props.baseProps}
                                    pagination={paginationFactory(PaginationOptions)}
                                    noDataIndication={noDataIndication}
                                    striped
                                />
                            </Fragment>
                        );
                    }
                }
            </ToolkitProvider>
        );
    }
}