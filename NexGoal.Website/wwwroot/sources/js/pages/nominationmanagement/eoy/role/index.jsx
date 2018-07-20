import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';

import commonFunctions from '../../../../common-functions';
import Filter from './Filter';
import Table from './Table';

export default class RoleManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoyId: document.getElementById('EOYId').value,
            filter: {
                keyword: '',
                groupId: '',
                roleId: '',
                isShowAll: false
            },
            nominators: [],
            resetDataTable: false,
            isHost: document.getElementById('IsHost').checked
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.editNominatorSetting = this.editNominatorSetting.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentWillMount() {
        this.getNominatorSetting();
    }

    getNominatorSetting() {
        axios.get(commonFunctions.getRootUrl()
            + `EOY/GetNominatorSetting?eoyId=${this.state.eoyId}`
            + `&keyword=${this.state.filter.keyword}`
            + `&groupIds=${this.state.filter.groupId}`
            + `&roleId=${this.state.filter.roleId}`
            + `&isShowAll=${this.state.filter.isShowAll}`)
            .then(response => {
                let data = response.data;
                data.map((e, index) => e.rowNum = index + 1);

                this.setState({ nominators: data });
            }).catch(commonFunctions.axiosError);
    }

    editNominatorSetting(nominator) {
        const setting = {
            eoqyId: this.state.eoyId,
            employeeId: nominator.employeeId,
            roleIds: nominator.roles.join(',')
        };

        let formData = commonFunctions.objectToFormData(setting);
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);

        axios({
            url: commonFunctions.getRootUrl() + 'EOY/EditNominatorSetting',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');

                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    handleFilterChange(filter) {
        this.setState({ filter: filter, resetDataTable: false });
    }

    handleSubmit() {
        this.setState({ resetDataTable: true });
        this.getNominatorSetting();
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    handleFilterChange={this.handleFilterChange}
                    handleFilterSubmit={this.handleSubmit} />

                <Table isHost={this.state.isHost}
                    resetDataTable={this.state.resetDataTable}
                    nominators={this.state.nominators}
                    editNominatorSetting={this.editNominatorSetting} />
            </Fragment>
        );
    }
}