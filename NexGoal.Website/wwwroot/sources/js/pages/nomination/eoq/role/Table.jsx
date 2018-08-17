import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';

import { PaginationOptions } from '../../../../constants';

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
            <BootstrapTable
                keyField='employeeId'
                noDataIndication={this.noDataDisplay}
                data={nominators}
                columns={columns}
                pagination={paginationFactory(PaginationOptions)}
                striped />
        );
    }
}

export default Table;