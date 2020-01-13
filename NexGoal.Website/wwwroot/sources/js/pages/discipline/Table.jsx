import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import moment from 'moment';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import isEqual from 'react-fast-compare';

import { PaginationOptions, TextCenterCssClass, PostRequestHeaders } from '../../constants';
import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

const { SearchBar } = Search;

export default class Table extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    handleEdit = (event, discipline) => {
        this.props.onEdit(discipline);
    }

    handleEditStatus = (event, discipline) => {
        const isActive = event.target.checked;

        axios({
            url: resolveClientUrl('discipline/editstatus'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: discipline.disciplineId,
                isActive: isActive
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
                this.props.onEditStatus(discipline.disciplineId, isActive);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const isHost = this.props.isHost;
        const columns = [
            {
                dataField: 'rowNum',
                isDummyField: true,
                text: `${getResource('SymbolCount')}`,
                classes: TextCenterCssClass,
                headerStyle: { width: '50px' },
                formatter: (cell, row, rowIndex) => rowIndex + 1
            },
            {
                dataField: 'employee.nickName',
                text: `${getResource('NickName')}`,
                headerStyle: { width: '120px' }
            },
            {
                dataField: 'employee.fullName',
                text: `${getResource('FullName')}`
            },
            {
                dataField: 'employee.groupName',
                text: `${getResource('Team')}`
            },
            {
                dataField: 'description',
                text: `${getResource('Content')}`
            },
            {
                dataField: 'effectiveDate',
                text: `${getResource('IssueDate')}`,
                classes: TextCenterCssClass,
                headerStyle: { width: '120px' },
                formatter: (cell, row) => `${moment(row.effectiveDate).format('DD MMM, YYYY')}`
            },
            {
                dataField: 'creator.nickName',
                text: `${getResource('Creator')}`,
                formatter: (cell, row) => `${row.creator.nickName} (${row.creator.fullName})`
            },
            {
                dataField: 'isActive',
                text: `${getResource('Active')}`,
                classes: TextCenterCssClass,
                headerStyle: { width: '80px' },
                formatter: (cell, row, rowIndex) => {
                    const idName = `chb-active-${rowIndex}`;

                    if (isHost) {
                        return (
                            <div className="checkbox">
                                <input type='checkbox' id={idName}
                                    defaultChecked={row.isActive}
                                    onChange={event => this.handleEditStatus(event, row)} className="checkbox-custom"
                                />
                                <label htmlFor={idName} />
                            </div>
                        );
                    }

                    return (
                        <div className="checkbox">
                            <input type='checkbox' id={idName}
                                checked={row.isActive} readOnly className="checkbox-custom"
                            />
                            <label htmlFor={idName} />
                        </div>
                    );
                }
            },
            {
                dataField: '',
                text: `${getResource('Edit')}`,
                classes: TextCenterCssClass,
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
                                                {getResource('AddDiscipline')}
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