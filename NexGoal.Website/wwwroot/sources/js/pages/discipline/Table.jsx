import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import moment from 'moment';
import axios from 'axios';
import { stringify } from 'qs';
import commonFunctions from '../../common-functions';
import { Form } from 'formik';
import { toast } from 'react-toastify';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.tableRef = React.createRef();
    }

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    editDisciplineStatus = (e, discipline) => {
        var isActive = e.target.checked;

        axios({
            url: '/discipline/editstatus',
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
                    if (this.props.canEdit) {
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
                    if (this.props.canEdit) {
                        return (
                            <a className="fa fa-pencil-square-o" title="Edit" onClick={() => this.props.editDiscipline(row)}></a>
                        );
                    }

                    return '';
                }
            }];

        return (
            <BootstrapTable noDataIndication={this.noDataDisplay} ref={this.tableRef} keyField='disciplineId' data={this.props.data} columns={columns} />
        );
    }
}