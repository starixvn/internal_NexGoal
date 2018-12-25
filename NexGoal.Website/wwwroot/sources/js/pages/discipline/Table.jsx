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
    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)
            || this.props.isHost !== nextProps.isHost) {
            return true;
        }

        return false;
    }

    handleEdit = (event, discipline) => {
        this.props.onEdit(discipline);
    }

    handleEditStatus = (event, discipline) => {
        let isActive = event.target.checked;

        axios({
            url: resolveClientUrl('discipline/editstatus'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: discipline.disciplineId,
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
                this.props.onEditStatus(discipline.disciplineId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        let isHost = this.props.isHost;
        let columns = [
            {
                dataField: 'rowNum',
                isDummyField: true,
                text: `${getResource('SymbolCount')}`,
                classes: 'text-center',
                headerStyle: { width: '50px' },
                formatter: (cell, row, rowIndex) => {
                    return rowIndex + 1;
                }
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
                dataField: 'groupName',
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
                formatter: (cell, row) => {
                    return `${moment(row.effectiveDate).format('DD MMM, YYYY')}`;
                }
            },
            {
                dataField: 'creatorFullName',
                text: `${getResource('Creator')}`,
                formatter: (cell, row) => {
                    return `${row.creatorFullName} (${row.creatorNickName})`;
                }
            },
            {
                dataField: 'isActive',
                text: `${getResource('Active')}`,
                classes: 'text-center',
                headerStyle: { width: '80px' },
                formatter: (cell, row) => {
                    if (isHost) {
                        return <input type='checkbox' defaultChecked={row.isActive} onChange={event => this.handleEditStatus(event, row)} />;
                    }

                    return <input type='checkbox' checked={row.isActive} readOnly />;
                }
            },
            {
                dataField: '',
                text: `${getResource('Edit')}`,
                classes: 'text-center',
                headerStyle: { width: '80px' },
                isDummyField: true,
                formatter: (cell, row) => {
                    if (isHost) {
                        return <a className="fa fa-pencil-square-o" onClick={event => this.handleEdit(event, row)} />;
                    }

                    return '';
                }
            }
        ];

        return (
            <ToolkitProvider keyField="disciplineId" data={this.props.data} columns={columns} search={{ searchFormatted: true }} >
                {
                    props => {
                        return (
                            <Fragment>
                                <div className="search-wrapper clearfix">
                                    <div className="pull-left">
                                        {this.props.isHost &&
                                            <button className="btn btn-primary" onClick={this.props.onCreate}>
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