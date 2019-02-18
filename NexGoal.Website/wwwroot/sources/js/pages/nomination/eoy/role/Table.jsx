import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';

import { noDataIndication } from '../../../../common-functions';
import { PaginationOptions } from '../../../../constants';
import getResource from '../../../../resourceHelper';

const { SearchBar } = Search;

class Table extends Component {
    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.nominators !== nextProps.nominators
            || this.props.isHost !== nextProps.isHost) {
            return true;
        }

        return false;
    }

    handleCheckboxChange = (event, nominator) => {
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

        this.props.onEdit(nominator);
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
                    return rowIndex + 1;
                }
            },
            { dataField: 'nickName', text: `${getResource('NickName')}`, headerStyle: { width: '120px' } },
            { dataField: 'fullName', text: `${getResource('FullName')}` },
            { dataField: 'jobTitle', text: `${getResource('JobTitle')}` },
            { dataField: 'groupName', text: `${getResource('Team')}` },
            {
                dataField: 'isCommittee',
                headerStyle: { width: '120px' },
                text: `${getResource('Committee')}`,
                classes: 'text-center',
                formatter: (cell, row, rowIndex) => {
                    if (isHost) {
                        return (
                            <div className="checkbox">
                            <input type='checkbox' name="Committee" className="checkbox-custom" id={'chb-committee' + rowIndex}
                                defaultChecked={row.isCommittee}
                                onChange={(e) => this.handleCheckboxChange(e, row)}
                            />
                            <label htmlFor={'chb-committee' + rowIndex}></label></div>
                        );
                    }

                    return <div className="checkbox"><input type='checkbox' className="checkbox-custom" id={'chb-committee' + rowIndex} name="Committee" defaultChecked={row.isCommittee} readOnly /><label htmlFor={'chb-committee' + rowIndex}></label></div>;
                }
            },
            {
                dataField: 'isNominator',
                text: `${getResource('Nominator')}`,
                classes: 'text-center',
                headerStyle: { width: '120px' },
                formatter: (cell, row, rowIndex) => {
                    if (isHost) {
                        return (
                            <div className="checkbox">
                                <input type='checkbox' name="Nominator" id={'chb-nominator' + rowIndex}
                                    defaultChecked={row.isNominator}
                                    onChange={(e) => this.handleCheckboxChange(e, row)} className="checkbox-custom"
                                />
                            <label htmlFor={'chb-nominator' + rowIndex}></label></div>
                        );
                    }

                    return <div className="checkbox"><input type='checkbox' id={'chb-nominator' + rowIndex} name="Nominator" defaultChecked={row.isNominator} className="checkbox-custom" readOnly /><label htmlFor={'chb-nominator' + rowIndex} ></label></div>;
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