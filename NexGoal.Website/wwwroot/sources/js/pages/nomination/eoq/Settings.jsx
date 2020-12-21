import React, { Component } from 'react';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, axiosEditResponseWithFormik } from '../../../common-functions';
import { validateEOQForm } from '../common';
import { PostRequestHeaders } from '../../../constants';
import EditNominationPeriodForm from '../EditNominationPeriodForm';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoq: {
                eoqyId: 0,
                quarter: 1,
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

    getEOQById = () => {
        const eoqId = document.getElementById('EOQId').value;
        axios.get(resolveClientUrl(`EOQ/GetById/${eoqId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    eoq: {
                        eoqyId: data.eoqyId,
                        quarter: data.quarter,
                        year: data.year,
                        startDateForNominator: moment(data.startDateForNominator),
                        endDateForNominator: moment(data.endDateForNominator),
                        remindDateForNominator: moment(data.remindDateForNominator),
                        endDateForCommittee: moment(data.endDateForCommittee),
                        description: data.description || ''
                    }
                });
            }).catch(axiosError);
    };

    handleSubmit = (values, { setErrors }) => {
        const eoq = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqId: this.state.eoq.eoqyId,
            quarter: values.quarter,
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOQ/Edit'),
            method: 'POST',
            data: eoq,
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
            <EditNominationPeriodForm isEOQ isHost={this.isHost}
                values={values} errors={errors} touched={touched}
                setFieldValue={setFieldValue}
                onChange={handleChange}
                onSubmit={handleSubmit}
            />
        );
    };

    render() {
        return (
            <Formik enableReinitialize initialValues={this.state.eoq}
                validate={validateEOQForm}
                onSubmit={this.handleSubmit}
                render={this.renderForm}
            />
        );
    }
}

export default Settings;