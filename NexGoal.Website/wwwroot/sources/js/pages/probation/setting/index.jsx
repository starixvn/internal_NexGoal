import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './Table';
import EditSetting from './EditSetting';
import AddEmployee from './AddEmployee';

export default class Setting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationers: null,
            showEditSettingModal: false,
            selectedProbationer: null,
            addNewEmployee: {
                showModal: false,
                employeeOptions: []
            }
        };

        this.employeeOptions = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('common/GetEmployees')),
            axios(resolveClientUrl('Probation/GetProbationers'))
        ];

        Promise.all(promises)
            .then(([employeeResponse, probationersResponse]) => {
                this.employeeOptions = employeeResponse.data.map(e => ({
                    value: e.employeeId, label: `${e.nickName} (${e.fullName})`
                }));

                this.setState({
                    probationers: probationersResponse.data
                });
            }).catch(axiosError);
    }

    getProbationers = () => {
        axios.get(resolveClientUrl('Probation/GetProbationers')).then(response => {
            this.setState({
                probationers: response.data
            });
        }).catch(axiosError);
    }

    onEditSetting = (event) => {
        const employeeId = event.target.dataset.employeeid;

        this.setState(prevState => ({
            showEditSettingModal: true,
            selectedProbationer: prevState.probationers.find(x => x.employeeId == employeeId)
        }));
    }

    handleCloseEditSettingModal = () => this.setState({ showEditSettingModal: false })

    handleEditSuccess = () => {
        this.setState({ showEditSettingModal: false });
        this.getProbationers();
    }

    handleShowAddEmployeeModal = () => {
        axios.get(resolveClientUrl('Probation/GetProbationersNotInEvaluation')).then(response => {
            this.setState({
                addNewEmployee: {
                    showModal: true,
                    employeeOptions: response.data.map(item => {
                        return {
                            value: item.employeeId,
                            label: `${item.nickName} (${item.fullName})`
                        };
                    })
                }
            });
        }).catch(axiosError);
    }

    handleCloseAddEmployeeModal = () => this.setState({ addNewEmployee: { showModal: false } })

    handleAddEmployeeSuccess = () => {
        this.handleCloseAddEmployeeModal();
        this.getProbationers();
    }

    render() {
        return (
            <div className="row" >
                <Breadcrumb>
                    <li className="current">{getResource('ProbationSetting')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Table tableData={this.state.probationers} onEditSetting={this.onEditSetting} onAddNewEmployee={this.handleShowAddEmployeeModal} />
                </div>
                {this.state.showEditSettingModal &&
                    <EditSetting onCloseModal={this.handleCloseEditSettingModal} probationer={this.state.selectedProbationer}
                        showModal employeeOptions={this.employeeOptions} onEditSuccess={this.handleEditSuccess} />
                }
                <AddEmployee showModal={this.state.addNewEmployee.showModal}
                    employeeOptions={this.state.addNewEmployee.employeeOptions}
                    onCloseModal={this.handleCloseAddEmployeeModal}
                    onAddEmployeesuccess={this.handleAddEmployeeSuccess}
                />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<Setting />, document.getElementById('probation-setting-wrapper'));