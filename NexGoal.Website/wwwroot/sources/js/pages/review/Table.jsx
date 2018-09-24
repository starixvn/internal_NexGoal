import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';

import getResource from '../../resourceHelper';
import { noDataIndication } from '../../common-functions';
import { PaginationOptions, PAStatus, PerformanceProcess } from '../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    columns = [
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
            dataField: 'employee.nickName',
            text: `${getResource('NickName')}`
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
            dataField: 'evaluator',
            text: `${getResource('Evaluator')}`,
            isDummyField: true,
            formatter: (cell, row) => {
                let evaluatorProcess = row.processes.find(p => p.processId === PerformanceProcess.Evaluator);

                return (
                    <Fragment>{`${evaluatorProcess.evaluator.fullName} (${evaluatorProcess.evaluator.nickName})`}</Fragment>
                );
            }
        },
        {
            dataField: 'statusId',
            text: `${getResource('Action')}`,
            classes: 'text-center',
            headerStyle: { width: '80px' },
            formatter: (cell, row) => {
                switch (row.statusId) {
                    case PAStatus.InProgress:
                        return <a className="fa fa-pencil-square-o" title="Edit" />;
                    case PAStatus.Closed:
                        return <a className="fa fa-search" title="Details" />;
                    default:
                        return <Fragment />;
                }
            }
        }]

    render() {
        return (
            <Fragment>
                <ToolkitProvider keyField="paRecordId" columns={this.columns} data={this.props.data} search={{ searchFormatted: true }} >
                    {
                        props => {
                            return (
                                <Fragment>
                                    <div className="search-wrapper clearfix" >
                                        <div className="search pull-right">
                                            <Search.SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                            <span className="fa fa-search icon" />
                                        </div>
                                    </div>
                                    <BootstrapTable {...props.baseProps} noDataIndication={noDataIndication}
                                        pagination={paginationFactory(PaginationOptions)} striped
                                    />
                                </Fragment>
                            );
                        }
                    }
                </ToolkitProvider>
            </Fragment>
        );
    }
}