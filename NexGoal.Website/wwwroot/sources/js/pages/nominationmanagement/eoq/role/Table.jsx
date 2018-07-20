import React, { Component } from 'react';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';

class Table extends Component {
    constructor(props) {
        super(props);

        this.tableRef = React.createRef();

        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
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
        const resetDataTable = this.props.resetDataTable;
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

        const paginationOptions = paginationFactory({
            page: this.tableRef.current && !resetDataTable ? this.tableRef.current.store._page : 1,
            sizePerPage: this.tableRef.current && !resetDataTable ? this.tableRef.current.store._sizePerPage : 20,
            showTotal: false,
            sizePerPageList: [
                { text: '20', value: 20 },
                { text: '50', value: 50 },
                { text: '100', value: 100 },
                { text: '200', value: 200 }
            ]
        });

        return (
            <BootstrapTable
                noDataIndication={this.noDataDisplay}
                ref={this.tableRef}
                keyField='employeeId'
                data={nominators}
                columns={columns}
                pagination={paginationOptions}
                striped />
        );
    }
}

export default Table;