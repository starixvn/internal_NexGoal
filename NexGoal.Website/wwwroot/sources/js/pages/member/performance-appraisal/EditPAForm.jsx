import React, { Component } from 'react';
import { Button, ControlLabel, Modal, FormGroup, Tab, Tabs } from 'react-bootstrap';
import Select from 'react-select';
import { camelCase, isEmpty } from 'lodash';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import moment from 'moment';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';
import CustomScrollbars from '../../../shared/components/CustomScrollbars';
import getResource from '../../../resourceHelper';
import axios from 'axios';
import { axiosError, getCurrentUserInfo, resolveClientUrl } from '../../../common-functions';
import { PAStatus, DateFormat } from '../../../constants';
import PAProcessTab from '../../../shared/components/performance-appraisal/PAProcessTab';

export default class EditMemberPA extends Component {
    constructor(props) {
        super(props);
        this.state = {
            paRecord: {
                paRecordId: null,
                grade: null,
                comment: '',
                paStatus: null
            },
            employee: {},
            paProcesses: [],
            currentUserInfo: getCurrentUserInfo(),
            currentDate: moment(),
            errors: {}
        };
    }

    componentDidMount() {
        axios(resolveClientUrl(`PA/GetById/${this.props.paRecordId}`)).then(response => {
            if (response.data) {
                let pa = response.data;
                let grade = pa.isRevised ? pa.grade : pa.originalGrade;
                this.setState({
                    paRecord: {
                        paRecordId: pa.paRecordId,
                        grade: grade ? this.props.gradeOptions.find(o => o.value === grade.gradeId) : grade,
                        comment: pa.comment,
                        paStatus: pa.statusId
                    },
                    employee: pa.employee,
                    paProcesses: pa.processes
                });
            }
        }).catch(axiosError);
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = (event) => {
        let errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    validateForm = () => {
        let errors = {}, isValid = true;
        let paRecord = this.state.paRecord;

        if (!paRecord.comment || paRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        if (!paRecord.grade || paRecord.grade.value <= 0) {
            errors.grade = getResource('GradeRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit = (event) => {
        event.preventDefault();

        if (this.validateForm() && this.state.paRecord.paStatus === PAStatus.InProgress) {
            let record = this.state.paRecord;

            let paRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: record.paRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment
            });
            axios({
                url: resolveClientUrl('Review/Edit'),
                method: 'POST',
                data: paRecord,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('UpdatedSuccessfully'));

                    this.props.onSubmitSuccess();
                } else {
                    if (data.errors) {
                        let errors = {};
                        data.errors.map(error => {
                            errors[camelCase(error.key)] = error.errorMessage;
                        });

                        this.setState({ errors });
                    } else {
                        toast.error(data.message);
                    }
                }
            }).catch(axiosError);
        }
    }

    render() {
        let paRecord = this.state.paRecord;
        let errors = this.state.errors;
        let employees = [];
        employees.push(this.state.employee);
        const currentUserInfo = this.state.currentUserInfo;
        let title = this.state.paRecord.paStatus === PAStatus.Closed ?
            getResource('PerformanceAppraisal') :
            getResource('EditPerformanceAppraisal');
        title += " OF " + this.state.employee.nickName;
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit performance-appraisal"
            >
                <Modal.Header>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CustomScrollbars>
                        <div className="record record-your-mind">
                            <div className="avatar-user">
                                <img src={currentUserInfo.avatar} className="avatar" />
                                <div className="person-info">
                                    <span className="role-reviewer">{currentUserInfo.nickName}</span>
                                    <span className="icon-checkin" />
                                    <span className="role-member">{this.state.employee.nickName}</span>
                                    <div className="submit-date">{this.state.currentDate.format(DateFormat)}</div>
                                </div>
                            </div>
                            <div className="comment">
                                <form className="form-filter form-performance edit">
                                    <FormGroup>
                                        <ControlLabel>{getResource('Comment')}</ControlLabel>
                                        <TextareaFullscreenEditor
                                            className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                            name="comment"
                                            value={paRecord.comment}
                                            onChange={this.handleCommentChange}
                                            disabled={this.state.paRecord.paStatus === PAStatus.Closed}
                                        />
                                    </FormGroup>
                                    <FormGroup>
                                        <div className="grade">
                                            <ControlLabel>{getResource('Grade')}</ControlLabel>
                                            <Select name="grade" classNamePrefix="react-select"
                                                className={`react-select-container grade-field ${!isEmpty(errors) && errors.grade ? 'input-validation-error' : ''}`}
                                                placeholder="Select Grade"
                                                value={paRecord.grade}
                                                options={this.props.gradeOptions}
                                                isSearchable={false}
                                                onChange={this.handleGradeChange}
                                                isDisabled={this.state.paRecord.paStatus === PAStatus.Closed}
                                            />
                                        </div>
                                    </FormGroup>
                                </form>
                            </div>
                        </div>
                        <PAProcessTab processes={this.state.paProcesses} employees={employees} />
                    </CustomScrollbars>

                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>
                        {
                            parseInt(paRecord.paStatus) !== PAStatus.Closed ? getResource('Cancel') : getResource('Close')
                        }
                    </Button>
                    {
                        parseInt(paRecord.paStatus) !== PAStatus.Closed &&
                        <Button type="submit" bsStyle="primary" name="save"
                            onClick={this.handleSubmit}
                        >
                            {getResource('Submit')}
                        </Button>
                    }
                </Modal.Footer>
            </Modal>
        );
    }
}