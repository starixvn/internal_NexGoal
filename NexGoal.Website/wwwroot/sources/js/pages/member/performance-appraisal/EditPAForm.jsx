import React, { Component, Fragment } from 'react';
import { Button, ControlLabel, Modal, FormGroup } from 'react-bootstrap';
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
import { PAStatus, DateFormat, PerformanceProcess } from '../../../constants';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import PAProcessTab from '../../../shared/components/performance-appraisal/PAProcessTab';
import PerformanceGradeInfo from '../../../shared/components/PerformanceGradeInfo';

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
            errors: {},
            showGradeInfo: false
        };
    }

    componentDidMount() {
        axios(resolveClientUrl(`PA/GetById/${this.props.paRecordId}`)).then(response => {
            if (response.data) {
                let pa = response.data;
                this.setState({
                    paRecord: {
                        paRecordId: pa.paRecordId,
                        grade: pa.grade && pa.statusId !== PAStatus.Closed ? this.props.gradeOptions.find(o => o.value === pa.grade.gradeId) : pa.grade,
                        comment: pa.comment,
                        creator: pa.processes.filter(process => process.processId === PerformanceProcess.Reviewer)[0].evaluator,
                        modifiedDate: pa.modifiedDate,
                        paStatus: pa.statusId
                    },
                    employee: pa.employee,
                    paProcesses: pa.processes
                });
            }
        }).catch(axiosError);
    }

    handleGradeChange = selectedOption => {
        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = event => {
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

    handleSubmit = event => {
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

    renderViewPA = paRecord => {
        const data = {
            typeId: 1,
            roleId: PerformanceProcess.Reviewer,
            creator: paRecord.creator,
            createdDate: paRecord.modifiedDate ? paRecord.modifiedDate : null,
            comment: paRecord.comment,
            grade: paRecord.grade
        };
        return (
            <div className="record record-your-mind">
                <PerformanceRecord data={data} isHideProjectTag />
            </div>
        );
    }

    handleOpenGradeInfo = () => this.setState({ showGradeInfo: true })

    handleCloseGradeInfo = () => this.setState({ showGradeInfo: false })

    renderEditPA = paRecord => {
        let errors = this.state.errors;
        const currentUserInfo = this.state.currentUserInfo;

        return (
            <div className="record record-your-mind">
                <div className="avatar-user">
                    <img src={currentUserInfo.avatar} className="avatar" />
                    <div className="person-info">
                        <span className="role-reviewer">{currentUserInfo.nickName}</span>
                        
                    </div>
                </div>
                <div className="date-time"><div className="submit-date">{this.state.currentDate.format(DateFormat)}</div></div>
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
                                <ControlLabel>{getResource('Grade')} <a className="grade-info" onClick={this.handleOpenGradeInfo} /></ControlLabel>
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
        );
    }

    render() {
        let paRecord = this.state.paRecord;
        let employees = [];
        employees.push(this.state.employee);
        let title = this.state.paRecord.paStatus === PAStatus.Closed ?
            getResource('PerformanceAppraisal') :
            getResource('EditPerformanceAppraisal');
        title += ' OF ' + this.state.employee.nickName;
        return (
            <Fragment>
                <Modal keyboard={false} backdrop="static"
                    show={this.props.isShowModal} onHide={this.props.onCloseModal}
                    dialogClassName="custom-modal modal-md form-performance edit performance-appraisal"
                >
                    <Modal.Header>
                        <Modal.Title>{title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <CustomScrollbars>
                            {
                                paRecord.paStatus === PAStatus.Closed ? this.renderViewPA(paRecord) : this.renderEditPA(paRecord)
                            }
                            <PAProcessTab processes={this.state.paProcesses} employees={employees} />
                        </CustomScrollbars>

                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn-cancel" onClick={this.props.onCloseModal}>
                            {
                                parseInt(paRecord.paStatus, 10) !== PAStatus.Closed ? getResource('Cancel') : getResource('Close')
                            }
                        </Button>
                        {
                            parseInt(paRecord.paStatus, 10) !== PAStatus.Closed &&
                            <Button type="submit" bsStyle="primary" name="save"
                                onClick={this.handleSubmit}
                            >
                                {getResource('Submit')}
                            </Button>
                        }
                    </Modal.Footer>
                </Modal>
                <PerformanceGradeInfo showModal={this.state.showGradeInfo} performanceGradeInfos={this.props.performanceGradeInfos} onCloseModal={this.handleCloseGradeInfo} />
            </Fragment>
        );
    }
}