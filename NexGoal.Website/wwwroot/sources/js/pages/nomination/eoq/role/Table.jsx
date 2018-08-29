import React, { Component, Fragment } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';

import { PaginationOptions } from '../../../../constants';

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

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    render() {
        const isHost = this.props.isHost;
        const nominators = this.props.nominators;
        const columns = [
            { dataField: 'rowNum', text: '#', classes: 'text-center' },
            { dataField: 'nickName', text: 'Nick Name' },
            { dataField: 'fullName', text: 'Full Name' },
            { dataField: 'jobTitle', text: 'Job Title' },
            { dataField: 'groupName', text: 'Team' },
            {
                dataField: 'isCommittee',
                text: 'Committee',
                classes: 'text-center',
                formatter: (cell, row) => {
                    if (isHost) {
                        return (
                            <input type='checkbox' name="Committee"
                                defaultChecked={row.isCommittee}
                                onChange={(e) => this.handleCheckboxChange(e, row)} />
                        );
                    }

                    return (<input type='checkbox' name="Committee" checked={row.isCommittee} readOnly />);
                }
            },
            {
                dataField: 'isNominator',
                text: 'Nominator',
                classes: 'text-center',
                formatter: (cell, row) => {
                    if (isHost) {
                        return (<input type='checkbox' name="Nominator"
                            defaultChecked={row.isNominator}
                            onChange={(e) => this.handleCheckboxChange(e, row)} />);
                    }

                    return (<input type='checkbox' name="Nominator" checked={row.isCommittee} readOnly />);
                }
            }
        ];

        return (
            <ToolkitProvider keyField="employeeId" data={nominators} noDataIndication={this.noDataDisplay} columns={columns} search >
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
                                <BootstrapTable {...props.baseProps} pagination={paginationFactory(PaginationOptions)} striped />
                            </Fragment>
                        );
                    }
                }
            </ToolkitProvider>
        );
    }
}

export default Table;