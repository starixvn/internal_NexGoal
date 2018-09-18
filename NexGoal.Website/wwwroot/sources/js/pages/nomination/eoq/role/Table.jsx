import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';

import { noDataIndication } from '../../../../common-functions';
import { PaginationOptions } from '../../../../constants';
import getResource from '../../../../resourceHelper';

const { SearchBar } = Search;

class Table extends Component {
    constructor(props) {
        super(props);

        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.nominators !== nextProps.nominators
            || this.props.isHost !== nextProps.isHost) {
            return true;
        }

        return false;
    }

    handleCheckboxChange(event, nominator) {
        const name = event.target.name;

        if (name === 'Nominator') {
            nominator.isNominator = event.target.checked;

            if (nominator.isNominator) {
                nominator.roles.push(1);
            } else {
                nominator.roles.splice(nominator.roles.indexOf(1), 1);
            }
        } else {
            nominator.isCommittee = event.target.checked;

            if (nominator.isCommittee) {
                nominator.roles.push(2);
            } else {
                nominator.roles.splice(nominator.roles.indexOf(2), 1);
            }
        }

        this.props.editNominatorSetting(nominator);
    }

    render() {
        const isHost = this.props.isHost;
        const nominators = this.props.nominators;
        const columns = [
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
            { dataField: 'nickName', text: `${getResource('NickName')}`, headerStyle: { width: '120px' } },
            { dataField: 'fullName', text: `${getResource('FullName')}` },
            { dataField: 'jobTitle', text: `${getResource('JobTitle')}` },
            { dataField: 'groupName', text: `${getResource('Team')}` },
            {
                dataField: 'isCommittee',
                text: `${getResource('Committee')}`,
                classes: 'text-center',
                headerStyle: { width: '120px' },
                formatter: (cell, row) => {
                    if (isHost) {
                        return (
                            <input type='checkbox' name="Committee"
                                defaultChecked={row.isCommittee}
                                onChange={(e) => this.handleCheckboxChange(e, row)}
                            />
                        );
                    }

                    return <input type='checkbox' name="Committee" checked={row.isCommittee} readOnly />;
                }
            },
            {
                dataField: 'isNominator',
                text: `${getResource('Nominator')}`,
                classes: 'text-center',
                headerStyle: { width: '120px' },
                formatter: (cell, row) => {
                    if (isHost) {
                        return (
                            <input type='checkbox' name="Nominator"
                                defaultChecked={row.isNominator}
                                onChange={(e) => this.handleCheckboxChange(e, row)}
                            />
                        );
                    }

                    return <input type='checkbox' name="Nominator" checked={row.isCommittee} readOnly />;
                }
            }
        ];

        return (
            <ToolkitProvider keyField="employeeId" data={nominators} columns={columns} search >
                {
                    props => {
                        return (
                            <Fragment>
                                <div className="search-wrapper clearfix">
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

export default Table;