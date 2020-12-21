import React, { Component } from 'react';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, axiosEditResponseWithFormik } from '../../../common-functions';
import { validateEOYForm } from '../common';
import { PostRequestHeaders } from '../../../constants';
import EditNominationPeriodForm from '../EditNominationPeriodForm';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoy: {
                eoqyId: 0,
                year: new Date().getFullYear(),
                startDateForNominator: null,
                endDateForNominator: null,
                remindDateForNominator: null,
                endDateForCommittee: null,
                description: ''
            }
        };

        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        this.getEOQById();
    }

    getEOQById() {
        const eoyId = document.getElementById('EOYId').value;

        axios.get(resolveClientUrl(`EOY/GetById/${eoyId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    eoy: {
                        eoqyId: data.eoqyId,
                        year: data.year,
                        startDateForNominator: moment(data.startDateForNominator),
                        endDateForNominator: moment(data.endDateForNominator),
                        remindDateForNominator: moment(data.remindDateForNominator),
                        endDateForCommittee: moment(data.endDateForCommittee),
                        description: data.description
                    }
                });
            }).catch(axiosError);
    }

    handleSubmit = (values, { setErrors }) => {
        const eoy = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoyId: this.state.eoy.eoqyId,
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOY/Edit'),
            method: 'POST',
            data: eoy,
            headers: PostRequestHeaders
        }).then(response => {
            axiosEditResponseWithFormik(setErrors, toast, response);
        }).catch(axiosError);
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <EditNominationPeriodForm isEOQ={false} isHost={this.isHost}
                values={values} errors={errors} touched={touched}
                setFieldValue={setFieldValue}
                onChange={handleChange}
                onSubmit={handleSubmit}
            />
        );
    };

    render() {
        return (
            <Formik enableReinitialize initialValues={this.state.eoy}
                validate={validateEOYForm}
                onSubmit={this.handleSubmit}
                render={this.renderForm}
            />
        );
    }
}

export default Settings;