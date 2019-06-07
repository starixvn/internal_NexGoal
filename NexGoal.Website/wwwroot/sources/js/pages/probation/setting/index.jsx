import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './Table';
import EditSetting from './EditSetting';

export default class Setting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationers: null,
            showEditSettingModal: false,
            selectedProbationer: null
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

    onEditSetting = () => {
        this.setState(prevState => ({
            showEditSettingModal: true,
            selectedProbationer: prevState.probationers.find(x => x.employeeId = event.target.dataset.employeeid)
        }));
    }

    handleCloseEditSettingModal = () => this.setState({ showEditSettingModal: false })

    handleEditSuccess = () => {
        this.setState({ showEditSettingModal: false });
        this.getProbationers();
    }

    render() {
        return (
            <div className="row" >
                <Breadcrumb>
                    <li className="current">Probation Setting</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Table tableData={this.state.probationers} onEditSetting={this.onEditSetting} />
                </div>
                {this.state.showEditSettingModal &&
                    <EditSetting onCloseModal={this.handleCloseEditSettingModal} probationer={this.state.selectedProbationer}
                        showModal employeeOptions={this.employeeOptions} onEditSuccess={this.handleEditSuccess} />
                }
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<Setting />, document.getElementById('probation-setting-wrapper'));